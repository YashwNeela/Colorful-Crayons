using UnityEngine;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Everything that makes one Bridge Quest mission: who is stuck, where they are
    /// trying to get to, the two storyboards that bookend the run, and the five
    /// questions that build the bridge.
    ///
    /// This is the piece that keeps the scene light. RocketRun hand-places one
    /// opening and one closing cut-scene in the scene and authors the panels in the
    /// inspector; six missions would mean twelve such objects. Here a single
    /// BridgeStoryCutsceneUI is repopulated from whichever MissionData is active,
    /// so adding mission seven is a new asset, not a new prefab branch.
    /// </summary>
    [CreateAssetMenu(
        fileName = "Mission_",
        menuName = "TMKOC/Bridge Quest/Mission",
        order = 0)]
    public class MissionData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("1-6. Display only -- running order comes from the LevelManager list.")]
        public int missionNumber = 1;

        [Tooltip("Tapu, Bhide, Abdul, Goli, Sonu, Gogi.")]
        public string characterName;

        [Tooltip("Playground, School, Gokuldham Society, Picnic Spot, Library, Puppy.")]
        public string destinationName;

        [Header("Art")]
        public Sprite characterPortrait;
        public Sprite backgroundArt;

        [Tooltip("One plank sprite, repeated five times as the bridge builds.")]
        public Sprite plankSprite;
        [Tooltip("Optional -- one sprite per bridge slot, for planks drawn in perspective.\n" +
                 "Any slot left empty here falls back to plankSprite above.")]
        public Sprite[] plankSprites;


        [Header("Storyboards")]
        [Tooltip("Plays before the questions -- the character's problem.")]
        public StoryPanel[] openingPanels;

        [Tooltip("Plays after the last plank lands -- the character crosses and arrives.")]
        public StoryPanel[] endingPanels;

        [Header("Questions")]
        [Tooltip("Five, per the GDD. One correct answer lays one plank.")]
        public QuestionData[] questions = new QuestionData[5];

        [Header("Voice-over")]
        [Tooltip("Spoken as the mission opens, after the storyboard. e.g. m1_mission_intro")]
        public string missionIntroVoiceKey;

        [Tooltip("Spoken over the final crossing. e.g. m1_mission_complete")]
        public string missionCompleteVoiceKey;

        /// <summary>How many planks this mission's bridge needs. Always the question count.</summary>
        public int PlankCount
        {
            get { return questions != null ? questions.Length : 0; }
        }

        /// <summary>
        /// Editor-time sanity check. Catches the two mistakes that are invisible until
        /// a child hits them: a question with no voice line (unreadable for the target
        /// age) and a correctIndex pointing outside the option list.
        /// </summary>
        public bool Validate(out string problem)
        {
            problem = null;

            if (questions == null || questions.Length == 0)
            {
                problem = name + ": no questions.";
                return false;
            }

            for (int i = 0; i < questions.Length; i++)
            {
                QuestionData q = questions[i];

                if (q == null)
                {
                    problem = name + ": question " + (i + 1) + " is null.";
                    return false;
                }

                if (!q.IsValid)
                {
                    problem = name + ": question " + (i + 1) + " has a correctIndex outside its options.";
                    return false;
                }

                if (string.IsNullOrEmpty(q.promptVoiceKey))
                {
                    problem = name + ": question " + (i + 1) + " has no promptVoiceKey -- a pre-reader cannot answer it.";
                    return false;
                }
            }

            return true;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            string problem;
            if (!Validate(out problem) && !string.IsNullOrEmpty(problem))
            {
                Debug.LogWarning("[BridgeQuest] " + problem, this);
            }
        }
#endif
    }
}
