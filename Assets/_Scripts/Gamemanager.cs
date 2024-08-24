using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Colorful_Crayons
{
    public enum GameState
    {
        Start,
        Playing, 
        Paused,
         GameOver,

         Win,
         Loose
    }
    public class Gamemanager : Singleton<Gamemanager>
    {

        #region  Game States

        protected GameState m_CurrentGameState;

        public GameState CurrentGameState => m_CurrentGameState;
        public static UnityAction OnGameWin;

        public static UnityAction OnGameLoose;

        public static UnityAction OnGameStart;

        public static UnityAction OnGamePlaying;

        public static UnityAction OnGamePaused;

        public static UnityAction OnGameOver;

        public virtual void GameStart()
        {
            m_CurrentGameState = GameState.Start;
            OnGameStart?.Invoke();
        }

        public virtual void GamePlaying()
        {
            m_CurrentGameState = GameState.Playing;

            OnGamePlaying?.Invoke();
        }

        public virtual void GamePaused()
        {
            m_CurrentGameState = GameState.Paused;

            OnGamePaused?.Invoke();
        }

        public virtual void GameOver()
        {
            m_CurrentGameState = GameState.GameOver;

            OnGameOver?.Invoke();
        }

        public virtual void GameWin()
        {
            m_CurrentGameState = GameState.Win;

            OnGameWin?.Invoke();
        }

        public virtual void GameLoose()
        {
            m_CurrentGameState = GameState.Loose;

            OnGameLoose?.Invoke();
        }

        #endregion
        
        public void Start()
        {
            GameStart();
        }
    }
}
