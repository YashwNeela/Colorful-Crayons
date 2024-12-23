using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC
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

        NotCompleted,

        Replay
    }
    public class GameManager : Singleton<GameManager>
    {
        public int GAMEID;
        public bool testLevel;

        public int levelNumber;


        protected GameCategoryDataManager m_CatergoryDataManager;
        public GameCategoryDataManager GameCategoryDataManager => m_CatergoryDataManager;

        protected UpdateCategoryApiManager m_UpdateCategoryApiManager;

        public UpdateCategoryApiManager UpdateCategoryApiManager => m_UpdateCategoryApiManager;


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

        public static UnityAction OnGameNotCompleted;

        public static UnityAction OnLoadNextLevel;

        public static UnityAction OnGameReplay;

        public static UnityAction OnLevelCompleteCheck;

         public void Start()
        {
            FirstTimeGameStart();
          
        }
        public virtual void FirstTimeGameStart()
        {
            m_CatergoryDataManager = new GameCategoryDataManager(GAMEID);
            m_UpdateCategoryApiManager = new UpdateCategoryApiManager(GAMEID);

            if (!testLevel)
            {
                levelNumber = m_CatergoryDataManager.GetCompletedLevel;
                if (levelNumber == LevelManager.Instance.MaxLevels)
                    levelNumber = 0;
            }
            else
                levelNumber = levelNumber;

            OnFirstTimeGameStartAction?.Invoke();

            

         StartCoroutine(StaticCoroutine.Co_GenericCoroutine(0, () =>
            {
                GameStart(levelNumber);

            }));


        }
        private void OnApplicationQuit()
        {
            // dataManager.SendData(()=>
            // {

            // });
            m_CatergoryDataManager.SaveLevel(LevelManager.Instance.CurrentLevelIndex, LevelManager.Instance.MaxLevels);
        }
        public virtual void GameStart(int level)
        {
            m_CurrentGameState = GameState.Start;
            LevelManager.Instance.LoadLevel(level);
            m_CatergoryDataManager.SaveLevel(level, LevelManager.Instance.MaxLevels);

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
            GameNotCompleted();
        }

        public virtual void GameNotCompleted()
        {
            m_CurrentGameState = GameState.NotCompleted;
            OnGameNotCompleted?.Invoke();
        }


        public virtual void GameLoose()
        {
            m_CurrentGameState = GameState.Loose;

            OnGameLoose?.Invoke();
        }

        public virtual void LoadNextLevel(int levelNo, float delay = 0)
        {
            StartCoroutine(Co_LoadNextLevel(levelNo, delay));

        }

        public virtual void ReplayGame()
        {
            OnGameReplay?.Invoke();
            LoadNextLevel(0);
        }

        #region GoBackToPlaySchool
        public virtual void GoBackToPlayschool()
        {

#if PLAYSCHOOL_MAIN
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

        private IEnumerator Co_LoadNextLevel(int levelNo, float delay)
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

            m_UpdateCategoryApiManager.SetGameDataMore(LevelManager.Instance.MaxLevels, LevelManager.Instance.MaxLevels, 0, 5);
            m_CatergoryDataManager.SaveLevel(LevelManager.Instance.MaxLevels, LevelManager.Instance.MaxLevels);
            OnGameCompleted?.Invoke();
        }

        public virtual void LevelCompleteCheck()
        {
            OnLevelCompleteCheck?.Invoke();
        }

    }
}
