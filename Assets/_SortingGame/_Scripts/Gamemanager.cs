using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using AssetKits.ParticleImage;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using System;
using System.Diagnostics;

namespace TMKOC.Sorting
{
    public enum GameState
    {
        FirstTimeGameStart,
        Start,
        Playing,
        Paused,
        GameOver,
        Restart,
        Win,
        Loose,
        Completed,

        Replay
    }


    public class Gamemanager : Singleton<Gamemanager>
    {
        public GameObject m_LevelCheckButton;
        [SerializeField] protected LevelManager m_LevelManager;

      //  protected DataManager dataManager;

     //   public DataManager DataManager => dataManager;

            protected GameCategoryDataManager m_CatergoryDataManager;
            public GameCategoryDataManager GameCategoryDataManager=> m_CatergoryDataManager;

            protected UpdateCategoryApiManager m_UpdateCategoryApiManager;

            public UpdateCategoryApiManager UpdateCategoryApiManager=> m_UpdateCategoryApiManager;

        public int GAMEID;

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

        [SerializeField] Vector3 m_LevelCompletedBlastOffset;
        [SerializeField] private ParticleSystem m_LevelCompletedBlast;

        [SerializeField] private Transform m_LevelCompleteBlastParent;

        public static UnityAction OnRightAnswerAction;
        public static UnityAction OnWrongAnswerAction;

     //   PlayschoolTestDataManager m_TestData;

        [Button]
        public void OpenTerminal()
        {
             string targetDirectory = "/Users/yash/Desktop/YashWUnityProjects/Colorful-Crayons";
            
            Process.Start("open", $"-a Terminal {targetDirectory}");
        }

        public void RightAnswer()
        {
            SetScore(m_CurrentScore + 1);
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

        public static UnityAction OnFirstTimeGameStartAction;
        public static UnityAction OnGameWin;

        public static UnityAction OnGameLoose;

        public static UnityAction OnGameStart;

        public static UnityAction OnGamePlaying;

        public static UnityAction OnGamePaused;

        public static UnityAction OnGameOver;

        public static UnityAction OnGameRestart;

        public static UnityAction OnGameCompleted;


        public static UnityAction OnLoadNextLevel;

        public static UnityAction OnGameReplay;

        public static UnityAction OnLevelCompleteCheck;

        public virtual void FirstTimeGameStart()
        {
            m_CatergoryDataManager = new GameCategoryDataManager(GAMEID);
            m_UpdateCategoryApiManager = new UpdateCategoryApiManager(GAMEID);

            if (!testLevel)
                levelNumber = m_CatergoryDataManager.GetCompletedLevel;
            // if(!testLevel)
            // dataManager.FetchData(() =>
            //     {
            //         GameStart(dataManager.StudentGameData.data.completedLevel);
            //     });

            GameStart(levelNumber);
                

            m_LevelCheckButton.GetComponentInChildren<Button>().interactable = false;

            Invoke(nameof(EnableLevelCheckButton),2);

            OnFirstTimeGameStartAction?.Invoke();

            
        }

        void EnableLevelCheckButton()
        {
            m_LevelCheckButton.GetComponentInChildren<Button>().interactable = true;
        }

        private void OnApplicationQuit() {
            // dataManager.SendData(()=>
            // {

            // });
            m_CatergoryDataManager.SaveLevel(LevelManager.Instance.CurrentLevelIndex,LevelManager.Instance.MaxLevels);
        }

        public virtual void LevelCompleteCheck()
        {
            OnLevelCompleteCheck?.Invoke();
        }
        public virtual void GameStart(int level)
        {
            m_CurrentGameState = GameState.Start;
            SetRemainingLives(m_MaxLives);
            SetScore(0);
            LevelManager.Instance.LoadLevel(level);
            m_CatergoryDataManager.SaveLevel(level,LevelManager.Instance.MaxLevels);

            int star = m_CatergoryDataManager.Getstar;
            if (star >= 5)
            {
                m_UpdateCategoryApiManager.SetGameDataMore(LevelManager.Instance.MaxLevels, LevelManager.Instance.MaxLevels, 0, 5);
            }
            else
            {
                m_UpdateCategoryApiManager.SetGameDataMore(level, LevelManager.Instance.MaxLevels, 0, star);
            }


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

            if (LevelManager.Instance.CurrentLevelIndex + 1 >= LevelManager.Instance.MaxLevels)
            {
                GameCompleted();
                return;
            }
            else
            {
               // dataManager.OnLevelCompleted();
                if (m_PlayCloudTransition)
                {
                    //CloudUI.Instance.PlayColoudEnterAnimation();
                }
                else
                {
                    ConfettiUI.Instance.PlayParticle();
                    Invoke(nameof(PlayLevelCompletedBlast), 2);
                }

            }
        }

        private void PlayLevelCompletedBlast()
        {
            ParticleSystem p = Instantiate(m_LevelCompletedBlast);
            p.transform.position = m_LevelCompleteBlastParent.position + m_LevelCompletedBlastOffset;

            p.Play();
            Gamemanager.Instance.LoadNextLevel(LevelManager.Instance.CurrentLevelIndex + 1);


        }

        public virtual void GameLoose()
        {
            m_CurrentGameState = GameState.Loose;

            OnGameLoose?.Invoke();
        }

        public virtual void LoadNextLevel(int levelNo,float delay = 0)
        {
            StartCoroutine(Co_LoadNextLevel(levelNo,delay));

        }

        public virtual void ReplayGame()
        {
            OnGameReplay?.Invoke();
            LoadNextLevel(0);
        }

        #region GoBackToPlaySchool
        public virtual void GoBackToPlayschool()
        {
        
            #if PLAYSHOOL_MAIN
           SceneManager.LoadScene(TMKOCPlaySchoolConstants.TMKOCPlayMainMenu);
            // dataManager.SendData(()=>
            #else
            UnityEngine.Debug.Log("Go back to playschool");
            #endif
            // {
            //     LoadSceneToMainMenu();
            // });
        }

        public void LoadSceneToMainMenu()
        {
            StartCoroutine(DelayHomeButton());
        }

        IEnumerator DelayHomeButton()
        {
           // if (AssetBundleLoading.instance != null)
            {
          //      AssetBundleLoading.instance.UnloadBundle();
            }
            yield return new WaitForSeconds(0.1f);
          //  SceneManager.LoadScene(TMKOCPlaySchoolConstants.TMKOCPlayMainMenu);
            Resources.UnloadUnusedAssets();
          //  dataManager.SendData();
            print("SENT");
            Destroy(gameObject);
        }

        private IEnumerator Co_LoadNextLevel(int levelNo,float delay)
        {
            yield return new WaitForSeconds(delay);
            GameStart(levelNo);
            OnLoadNextLevel?.Invoke();
        }

        #endregion

        public virtual void GameCompleted()
        {
            m_CurrentGameState = GameState.Completed;
        //    dataManager.SetCompletedLevel(dataManager.StudentGameData.data.totalLevel);
        //     dataManager.OnGameCompleted();
            
            m_UpdateCategoryApiManager.SetGameDataMore(LevelManager.Instance.MaxLevels,LevelManager.Instance.MaxLevels,0,5);
            m_CatergoryDataManager.SaveLevel(LevelManager.Instance.MaxLevels,LevelManager.Instance.MaxLevels);
            OnGameCompleted?.Invoke();
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
          //   dataManager = new DataManager(GAMEID, Time.time, m_LevelManager.MaxLevels,testLevel);
        }
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
           // DataManager.OnDataManagerInitialized += OnDataManagerInitialized;
        }

        private void OnDisable() {
            //DataManager.OnDataManagerInitialized -= OnDataManagerInitialized;
            
        }

        private void OnDataManagerInitialized()
        {
            FirstTimeGameStart();
        }



        public void Start()
        {
            FirstTimeGameStart();
          
        }

         private void Update() {
            if(Input.GetKeyDown(KeyCode.P))
            {
               // dataManager.SendData();
            }
        }


    }
}
