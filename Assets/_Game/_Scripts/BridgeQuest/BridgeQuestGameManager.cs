using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Bridge Quest's GameManager.
    ///
    /// Deliberately much thinner than RocketRunGameManager. RocketRun is one
    /// continuous procedural run with no discrete levels, so it had to override the
    /// lifecycle to strip out LevelManager indexing and the
    /// GameCategoryDataManager/UpdateCategoryApiManager star-saving. Bridge Quest is
    /// the opposite: six numbered missions IS the discrete level list the base class
    /// already expects, so the base implementations of FirstTimeGameStart, GameStart,
    /// GameWin and GameCompleted are all correct as-is and are left alone.
    ///
    /// In particular base GameWin() already does the right thing -- GameCompleted()
    /// on mission six, GameNotCompleted() on missions one to five.
    ///
    /// Bridge Quest is a no-fail game: a wrong tap costs nothing and the child may
    /// retry forever. GameLoose/GameOver are inherited but never called.
    /// </summary>
    public class BridgeQuestGameManager : GameManager
    {
        /// <summary>Fired when the child taps the correct answer.</summary>
        public static UnityAction<QuestionType> OnCorrectAnswer;

        /// <summary>Fired on a wrong tap. Informational only -- nothing is deducted.</summary>
        public static UnityAction<QuestionType> OnWrongAnswer;

        /// <summary>Fired each time a plank lands, with the 1-based plank number and the total.</summary>
        public static UnityAction<int, int> OnPlankPlaced;

        /// <summary>Fired when the fifth plank completes the span, before the crossing.</summary>
        public static UnityAction OnBridgeComplete;

        public static void RaiseCorrectAnswer(QuestionType t) { if (OnCorrectAnswer != null) OnCorrectAnswer(t); }
        public static void RaiseWrongAnswer(QuestionType t) { if (OnWrongAnswer != null) OnWrongAnswer(t); }
        public static void RaisePlankPlaced(int index, int total) { if (OnPlankPlaced != null) OnPlankPlaced(index, total); }
        public static void RaiseBridgeComplete() { if (OnBridgeComplete != null) OnBridgeComplete(); }

        /// <summary>
        /// Standalone escape hatch. The base FirstTimeGameStart reaches straight into
        /// LevelManager.Instance and the category/api managers, which only exist once
        /// the Playschool shell has set the game up. Ticking this in the inspector lets
        /// the scene be opened and played on its own without that whole chain -- exactly
        /// the situation BridgeQuestVoice already guards against on the audio side.
        /// </summary>
        [Header("Bridge Quest")]
        [Tooltip("Skip the shell's level/star bookkeeping so the scene can be played directly in the editor.")]
        [SerializeField] private bool standalone;

        public override void FirstTimeGameStart()
        {
            if (!standalone)
            {
                base.FirstTimeGameStart();
                return;
            }

            m_CurrentGameState = GameState.FirstTimeGameStart;
            levelNumber = 0;
            if (OnFirstTimeGameStartAction != null) OnFirstTimeGameStartAction();
        }

        public override void GameStart(int level)
        {
            if (!standalone)
            {
                base.GameStart(level);
                return;
            }

            m_CurrentGameState = GameState.Start;
            if (LevelManager.Instance != null) LevelManager.Instance.LoadLevel(level);
            if (OnGameStart != null) OnGameStart();
            GamePlaying();
        }

        public override void GameWin()
        {
            if (!standalone)
            {
                base.GameWin();
                return;
            }

            m_CurrentGameState = GameState.Win;
            if (OnGameWin != null) OnGameWin();

            if (LevelManager.Instance != null
                && LevelManager.Instance.CurrentLevelIndex + 1 >= LevelManager.Instance.MaxLevels)
            {
                GameCompleted();
                return;
            }
            GameNotCompleted();
        }

        public override void GameCompleted()
        {
            if (!standalone)
            {
                base.GameCompleted();
                return;
            }

            m_CurrentGameState = GameState.Completed;
            if (OnGameCompleted != null) OnGameCompleted();
        }
    }
}
