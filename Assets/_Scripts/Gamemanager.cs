using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

namespace TMKOC.Colorful_Crayons
{
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver,
        Restart,
        Win,
        Loose
    }


    public class Gamemanager : Singleton<Gamemanager>
    {
        protected LevelManager m_LevelManager;

        #region  Lives
        [SerializeField] private int m_MaxLives;

        public int MaxLives => m_MaxLives;

        private int m_RemaningLives;

        public int RemaningLives => m_RemaningLives;

        [SerializeField] private TextMeshProUGUI m_RemaningLivesText;

        private int m_CurrentScore;

        public int CurrentScore => m_CurrentScore;

        [SerializeField] private TextMeshProUGUI m_CurrentScoreText;

        public void RightAnswer()
        {
            SetScore(m_CurrentScore+1);
        }

        public void WrongAnswer()
        {

            if (m_RemaningLives - 1 >= 0)
            {
                SetRemainingLives(m_RemaningLives - 1);
                if (m_RemaningLives <= 0)
                {
                    GameOver();
                    GameLoose();
                }
            }
        }

        private void SetRemainingLives(int value)
        {
            m_RemaningLives = value;
            m_RemaningLivesText.text = m_RemaningLives.ToString();
        }

        private void SetScore(int value)
        {
            m_CurrentScore = value;
            m_CurrentScoreText.text = m_CurrentScore.ToString();
        }

        #endregion

        #region  Game States

        protected GameState m_CurrentGameState;

        public GameState CurrentGameState => m_CurrentGameState;
        public static UnityAction OnGameWin;

        public static UnityAction OnGameLoose;

        public static UnityAction OnGameStart;

        public static UnityAction OnGamePlaying;

        public static UnityAction OnGamePaused;

        public static UnityAction OnGameOver;

        public static UnityAction OnGameRestart;


        public virtual void GameStart()
        {
            SetRemainingLives(m_MaxLives);
            SetScore(0);

            LevelManager.Instance.LoadLevel(0);
            m_CurrentGameState = GameState.Start;
            OnGameStart?.Invoke();
        }

        public virtual void GameRestart()
        {
            m_CurrentGameState = GameState.Restart;
            OnGameRestart?.Invoke();
            GameStart();
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
