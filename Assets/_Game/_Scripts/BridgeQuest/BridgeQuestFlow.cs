using System;
using System.Collections;
using TMPro;
using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Owns one mission end to end:
    ///
    ///   opening storyboard -> tutorial (first mission only) -> five questions,
    ///   each correct answer dropping a plank -> confetti -> the crossing ->
    ///   closing storyboard -> win screen
    ///
    /// The ordering rules below are lifted from RocketRun, where they were learned
    /// the hard way:
    ///
    ///  - Only one voice line is ever in flight. The storyboard narrates, THEN the
    ///    tutorial speaks, THEN the first question is asked. Nothing is spoken while
    ///    a card is still moving.
    ///  - Every full-screen card freezes the world (Time.timeScale = 0) and animates
    ///    on unscaled time. Each card caches and restores the previous timescale
    ///    rather than assuming 1, so nested cards cannot strand the clock at zero.
    ///  - Replay is an in-place reset, never a scene reload, so the storyboard and
    ///    the tutorial do not replay.
    /// </summary>
    public class BridgeQuestFlow : MonoBehaviour
    {
        [Header("Mission")]
        [Tooltip("Fallback when no BridgeMission Level supplies one -- handy for testing a single mission in isolation.")]
        [SerializeField] private MissionData mission;

        [Header("Refs")]
        [SerializeField] private BridgeStoryCutsceneUI storyboard;
        [SerializeField] private QuestionCardUI questionCard;
        [SerializeField] private BridgeBuilderUI bridge;
        [SerializeField] private PlankCompletePopup plankPopup;
        [SerializeField] private BridgeQuestEndScreenUI winScreen;
        [SerializeField] private BridgeQuestTutorial tutorial;
        [SerializeField] private ConfettiUI winConfetti;

        [Tooltip("Adult-facing status line. The child is served by the voice-over, not by this.")]
        [SerializeField] private TextMeshProUGUI bannerText;

        [Header("Tuning")]
        [Tooltip("Beat between the plank landing and the next question, so the bridge growing is actually watched.")]
        [SerializeField] private float betweenQuestionsDelay = 0.4f;

        [Tooltip("Victory beat after the last plank, before the crossing starts.")]
        [SerializeField] private float preCrossingDelay = 0.8f;

        [Tooltip("Breath after the character arrives, before the closing storyboard takes over.")]
        [SerializeField] private float preClosingStoryDelay = 0.6f;

        [Header("Tutorial")]
        [Tooltip("Runs the opening lesson only on the first mission. Later missions go straight from storyboard to questions.")]
        [SerializeField] private bool tutorialOnFirstMissionOnly = true;

        private int questionIndex;
        private bool finished;
        private bool tutorialShown;        private bool missionStarted;


        /// <summary>The mission being played. Set by BridgeMission on level load.</summary>
        public MissionData Mission { get { return mission; } }

        private void Awake()
        {
            if (winConfetti == null) winConfetti = FindObjectOfType<ConfettiUI>();
            if (tutorial == null) tutorial = FindObjectOfType<BridgeQuestTutorial>();
        }

        private void Start()
        {
            // make sure this game's voice-over bundle is in memory even when the scene
            // is opened directly rather than through the Playschool menu
            BridgeQuestVoice.EnsureLoaded();

            // GameStart drives LevelManager.LoadLevel, which activates the first
            // BridgeMission, which calls BeginMission for us. Only fall back to the
            // inspector-assigned mission if that chain did NOT fire -- starting twice
            // would kill the first RunMission mid-storyboard and strand
            // BridgeStoryCutsceneUI.IsPlaying at true, deadlocking the tutorial.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameStart(GameManager.Instance.levelNumber);
            }

            // NB: nothing is spoken here. Start() runs underneath the opening
            // storyboard, so any line fired now would be talked over.
            if (!missionStarted) BeginMission(mission);
        }

        /// <summary>
        /// Starts a mission from the top. Called by Start for the inspector-assigned
        /// mission, and by BridgeMission.OnLevelLoaded for each mission in the list.
        /// </summary>
        public void BeginMission(MissionData data)
        {
            if (data == null)
            {
                Debug.LogWarning("[BridgeQuest] BeginMission called with no MissionData -- nothing to play.", this);
                return;
            }

            mission = data;
            missionStarted = true;
            questionIndex = 0;
            finished = false;

            if (questionCard != null) questionCard.Hide();
            if (bridge != null) bridge.ResetBridge(data.plankSprite);

            SetBanner("Help " + data.characterName + " reach the " + data.destinationName + "!");

            StopAllCoroutines();
            StartCoroutine(RunMission());
        }

        private IEnumerator RunMission()
        {
            // ---- 1. opening storyboard ----
            yield return PlayStoryboard(mission.openingPanels);

            // ---- 2. tutorial, first mission only ----
            bool wantTutorial = tutorial != null
                && (!tutorialOnFirstMissionOnly || !tutorialShown);

            if (wantTutorial)
            {
                tutorialShown = true;

                bool done = false;
                tutorial.RunOpeningLesson(delegate { done = true; });
                while (!done) yield return null;
            }

            // the storyboard has finished narrating and the lesson has stopped
            // speaking -- first moment in the whole opening where a line is safe
            BridgeQuestVoice.Play(mission.missionIntroVoiceKey);

            // ---- 3. the five questions ----
            AskNext();
        }

        private IEnumerator PlayStoryboard(StoryPanel[] panels)
        {
            if (storyboard == null || panels == null || panels.Length == 0) yield break;

            bool done = false;
            storyboard.Play(panels, delegate { done = true; });
            while (!done) yield return null;
        }

        private void AskNext()
        {
            if (finished || mission == null) return;

            if (mission.questions == null || questionIndex >= mission.questions.Length)
            {
                StartCoroutine(FinishRoutine());
                return;
            }

            QuestionData q = mission.questions[questionIndex];

            if (questionCard == null)
            {
                Debug.LogWarning("[BridgeQuest] No QuestionCardUI wired -- skipping straight to the bridge.", this);
                OnQuestionAnswered();
                return;
            }

            questionCard.Present(q, OnQuestionAnswered);

            // arm the stuck-nudge: if nothing is tapped for a while, point at the
            // answer. The GDD has no hint system at all, and a pre-reader who cannot
            // find the answer has no other way out of the question.
            if (tutorial != null) tutorial.ArmHint(questionCard);
        }

        private void OnQuestionAnswered()
        {
            if (tutorial != null) tutorial.DisarmHint();

            questionIndex++;

            if (bridge == null)
            {
                StartCoroutine(AfterPlank());
                return;
            }

            bridge.PlaceNextPlank(delegate { StartCoroutine(AfterPlank()); });
        }

        private IEnumerator AfterPlank()
        {
            bool lastPlank = mission != null
                && mission.questions != null
                && questionIndex >= mission.questions.Length;

            // the plank card is skipped on the last plank -- the confetti, the
            // crossing and the closing storyboard are the payoff there, and a card
            // in front of them just delays it
            if (!lastPlank && plankPopup != null)
            {
                bool dismissed = false;
                string msg = BuildPlankMessage();
                Sprite icon = mission != null ? mission.plankSprite : null;

                plankPopup.Show(msg, icon, delegate { dismissed = true; });
                while (!dismissed) yield return null;
            }

            yield return new WaitForSecondsRealtime(betweenQuestionsDelay);

            AskNext();
        }

        private string BuildPlankMessage()
        {
            if (bridge == null) return "Nice!";

            int left = bridge.Total - bridge.Placed;
            if (left <= 0) return "The bridge is ready!";
            if (left == 1) return "One more plank!";
            return left + " more planks!";
        }

        /// <summary>
        /// Whole bridge is built. Confetti over the HUD, the crossing, the closing
        /// storyboard, then the usual win flow.
        /// </summary>
        private IEnumerator FinishRoutine()
        {
            finished = true;

            SetBanner(mission.characterName + " can cross!");

            if (winConfetti != null) winConfetti.PlayParticle();
            BridgeQuestVoice.Play(mission.missionCompleteVoiceKey);

            yield return new WaitForSecondsRealtime(preCrossingDelay);

            // ---- the crossing: the thing the whole mission was for ----
            if (bridge != null)
            {
                bool arrived = false;
                bridge.PlayCrossing(delegate { arrived = true; });
                while (!arrived) yield return null;
            }

            yield return new WaitForSecondsRealtime(preClosingStoryDelay);

            // ---- closing storyboard ----
            yield return PlayStoryboard(mission.endingPanels);

            // base GameManager.GameWin() already routes correctly: GameCompleted() on
            // the last mission, GameNotCompleted() on every other -- no special-casing
            // needed here
            if (GameManager.Instance != null) GameManager.Instance.GameWin();

            if (winScreen != null)
            {
                winScreen.Show(HasNextMission() ? (Action)LoadNextMission : null, RestartMission);
            }
        }

        private bool HasNextMission()
        {
            LevelManager lm = LevelManager.Instance;
            if (lm == null) return false;
            return lm.CurrentLevelIndex + 1 < lm.MaxLevels;
        }

        private void LoadNextMission()
        {
            LevelManager lm = LevelManager.Instance;
            if (lm == null) return;

            // LoadNextLevel activates the next mission GameObject, whose BridgeMission
            // calls back into BeginMission -- so the whole reset happens there
            lm.LoadNextLevel();
        }

        /// <summary>
        /// Replays the current mission in place. Deliberately does NOT reload the
        /// scene and does NOT replay the opening storyboard or the tutorial -- same
        /// contract as RocketRun's RestartRun.
        /// </summary>
        public void RestartMission()
        {
            if (mission == null) return;

            questionIndex = 0;
            finished = false;

            if (questionCard != null) questionCard.Hide();
            if (bridge != null) bridge.ResetBridge(mission.plankSprite);

            SetBanner("Help " + mission.characterName + " reach the " + mission.destinationName + "!");

            StopAllCoroutines();
            AskNext();
        }

        private void SetBanner(string text)
        {
            if (bannerText != null) bannerText.text = text;
        }
    }
}
