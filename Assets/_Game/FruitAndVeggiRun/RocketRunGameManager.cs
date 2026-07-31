using UnityEngine;
using UnityEngine.Events;

namespace TMKOC
{
    /// <summary>
    /// RocketRun's GameManager. Shares the same lifecycle/event bus as the rest of the
    /// PlayschoolAPI games (GameManager.OnGameStart/OnGameWin/OnGameCompleted etc.), but
    /// RocketRun is one continuous procedural run with a shopping-list mission, not a
    /// discrete numbered level -- so every override below skips the LevelManager level-list
    /// indexing and the GameCategoryDataManager/UpdateCategoryApiManager star-saving calls
    /// that the base class performs unconditionally. Crashes stay purely cosmetic (the
    /// player just respawns), so GameLoose/GameOver are intentionally never called here --
    /// they're inherited but unused.
    /// </summary>
    public class RocketRunGameManager : GameManager
    {
        /// <summary>Fired when the player picks up the item currently being hunted.</summary>
        public static UnityAction OnCorrectPickup;

        /// <summary>Fired when the player picks up a produce item that isn't the current target.</summary>
        public static UnityAction OnIncorrectPickup;

        /// <summary>Fired when the player crashes into a hazard (purely cosmetic -- no lives lost).</summary>
        public static UnityAction OnPlayerCrashed;

        public static void RaiseCorrectPickup() { OnCorrectPickup?.Invoke(); }
        public static void RaiseIncorrectPickup() { OnIncorrectPickup?.Invoke(); }
        public static void RaisePlayerCrashed() { OnPlayerCrashed?.Invoke(); }

        public override void FirstTimeGameStart()
        {
            // Skip GAMEID / GameCategoryDataManager / UpdateCategoryApiManager entirely --
            // RocketRun isn't tracked in the discrete level/star progress system.
            m_CurrentGameState = GameState.FirstTimeGameStart;
            OnFirstTimeGameStartAction?.Invoke();
        }

        public override void GameStart(int level)
        {
            // No LevelManager.LoadLevel -- RocketRun has no level list to index into.
            m_CurrentGameState = GameState.Start;
            OnGameStart?.Invoke();
            GamePlaying();
        }

        public override void GameWin()
        {
            m_CurrentGameState = GameState.Win;
            OnGameWin?.Invoke();
            GameCompleted();
        }

        public override void GameCompleted()
        {
            m_CurrentGameState = GameState.Completed;
            OnGameCompleted?.Invoke();
        }
    }
}
