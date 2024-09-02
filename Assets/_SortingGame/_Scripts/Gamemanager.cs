using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using DG.Tweening;

namespace TMKOC.Sorting
{
    public enum GameState
    {
        Start,
        Playing,
        Paused,
        GameOver,
        Restart,
        Win,
        Loose,
        Completed
    }


    public class Gamemanager : Singleton<Gamemanager>
    {
        protected LevelManager m_LevelManager;

        public bool testLevel;

        public int levelNumber;

        public DOTweenAnimation m_CameraShakeDotweenAnimation;

        #region  Lives
        [SerializeField] private int m_MaxLives;

        public int MaxLives => m_MaxLives;

        private int m_RemaningLives;

        public int RemaningLives => m_RemaningLives;

        [SerializeField] private TextMeshProUGUI m_RemaningLivesText;

        private int m_CurrentScore;

        public int CurrentScore => m_CurrentScore;

        [SerializeField] private TextMeshProUGUI m_CurrentScoreText;

        public static UnityAction OnRightAnswerAction;
        public static UnityAction OnWrongAnswerAction;

        public void RightAnswer()
        {
            SetScore(m_CurrentScore+1);
            OnRightAnswerAction?.Invoke();
        }

        public void WrongAnswer()
        {

            if (m_RemaningLives - 1 >= 0)
            {
                m_CameraShakeDotweenAnimation.DOPlay();
                SetRemainingLives(m_RemaningLives - 1);
                OnWrongAnswerAction?.Invoke();
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

        [SerializeField] protected GameState m_CurrentGameState;

        public GameState CurrentGameState => m_CurrentGameState;

        public bool m_PlayCloudTransition;
        public static UnityAction OnGameWin;

        public static UnityAction OnGameLoose;

        public static UnityAction OnGameStart;

        public static UnityAction OnGamePlaying;

        public static UnityAction OnGamePaused;

        public static UnityAction OnGameOver;

        public static UnityAction OnGameRestart;

        public static UnityAction OnGameCompleted;


        public static UnityAction OnLoadNextLevel;


        public virtual void GameStart(int level)
        {
            m_CurrentGameState = GameState.Start;
            SetRemainingLives(m_MaxLives);
            SetScore(0);
            LevelManager.Instance.LoadLevel(level);
            OnGameStart?.Invoke();

            GamePlaying();

            
        }

        public virtual void GameRestart()
        {
            m_CurrentGameState = GameState.Restart;
            OnGameRestart?.Invoke();
            GameStart(LevelManager.Instance.CurrentLevelIndex);
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

            if(LevelManager.Instance.CurrentLevelIndex + 1 >= LevelManager.Instance.MaxLevels)
            {
                GameCompleted();
                return;
            }else
            {
                if (m_PlayCloudTransition)
                {
                    CloudUI.Instance.PlayColoudEnterAnimation();
                }
                else
                ConfettiUI.Instance.PlayParticle();

            }
        }

        public virtual void GameLoose()
        {
            m_CurrentGameState = GameState.Loose;

            OnGameLoose?.Invoke();
        }

        public virtual void LoadNextLevel(float delay = 0)
        {
            StartCoroutine(Co_LoadNextLevel(delay));

        }

        private IEnumerator Co_LoadNextLevel(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameStart(LevelManager.Instance.CurrentLevelIndex + 1);
            OnLoadNextLevel?.Invoke();
        }

        public virtual void GameCompleted()
        {
            m_CurrentGameState = GameState.Completed;
            OnGameCompleted?.Invoke();
        }

        #endregion

        public void Start()
        {
            if(testLevel)
            {
            GameStart(levelNumber);

            }else
                GameStart(0);
        }


    }
}
