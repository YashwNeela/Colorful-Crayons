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
        [Tooltip("The POOL this mission draws from -- not the run itself.\n" +
                 "Author as many as you like; each run takes questionsPerRun of them at random.")]
        public QuestionData[] questions = new QuestionData[5];

        [Tooltip("How many questions one run asks -- one plank each, so this is the bridge length.\n" +
                 "Clamped to the pool size, so a short pool simply asks everything it has.")]
        public int questionsPerRun = 5;

        [Header("Voice-over")]
        [Tooltip("Spoken as the mission opens, after the storyboard. e.g. m1_mission_intro")]
        public string missionIntroVoiceKey;

        [Tooltip("Spoken over the final crossing. e.g. m1_mission_complete")]
        public string missionCompleteVoiceKey;

        /// <summary>
        /// How many planks this mission's bridge needs -- the length of one run, not
        /// the size of the pool. Reading the pool here would build a ten-plank bridge
        /// for a five-question run.
        /// </summary>
        public int PlankCount
        {
            get
            {
                if (questions == null) return 0;
                return Mathf.Clamp(questionsPerRun, 0, questions.Length);
            }
        }

        /// <summary>
        /// Draws one run's worth of questions from the pool, in random order and
        /// without repeats. Called once when a mission begins (and again on replay),
        /// never per question -- re-rolling mid-run would change the bridge length
        /// underneath a run already in progress.
        ///
        /// Nulls are filtered first so a half-filled inspector array cannot hand the
        /// flow a null question, which would read as a frozen card to the child.
        /// </summary>
        public QuestionData[] BuildRun()
        {
            if (questions == null || questions.Length == 0) return new QuestionData[0];

            System.Collections.Generic.List<QuestionData> pool =
                new System.Collections.Generic.List<QuestionData>(questions.Length);

            for (int i = 0; i < questions.Length; i++)
                if (questions[i] != null) pool.Add(questions[i]);

            if (pool.Count == 0) return new QuestionData[0];

            int take = Mathf.Clamp(questionsPerRun, 0, pool.Count);
            if (take == 0) return new QuestionData[0];

            // partial Fisher-Yates: shuffle only the first 'take' slots
            for (int i = 0; i < take; i++)
            {
                int j = Random.Range(i, pool.Count);
                QuestionData tmp = pool[i];
                pool[i] = pool[j];
                pool[j] = tmp;
            }

            QuestionData[] run = new QuestionData[take];
            for (int i = 0; i < take; i++) run[i] = pool[i];
            return run;
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
