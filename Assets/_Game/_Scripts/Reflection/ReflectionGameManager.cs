using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionGameManager : GameManager
    {


        public override void Start()
        {
            StartIntroCutScene();
        }
        public override void FirstTimeGameStart()
        {
            m_CurrentGameState = GameState.FirstTimeGameStart;
            m_CatergoryDataManager = new GameCategoryDataManager(GAMEID, PlayerPrefs.GetString("currentGameName"));
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
            GameStart(levelNumber);
            
        }

        public override void StartIntroCutScene()
        {
            base.StartIntroCutScene();
            CinematicCutSceneManager.Instance.StartCutScene(1, () =>
            {
                Debug.Log("Cut scene ended");
                PlaySplashScreen();

                StartCoroutine(StaticCoroutine.Co_GenericCoroutine(1, () =>
                {
                    //GameStart(levelNumber);
                }));
            });

        }

        public override void StartOutroCutScene()
        {
            base.StartOutroCutScene();
            CinematicCutSceneManager.Instance.StartCutScene(2, () =>
            {
                Debug.Log("outro Cut scene ended");
                

                
            });
        }

        public override void GameWin()
        {
            m_CurrentGameState = GameState.Win;
            OnGameWin?.Invoke();
        }



        public override void PlaySplashScreen()
        {
            base.PlaySplashScreen();
            SplashScreenUI.Instance.EnableSplashScreen();
        }

    }


}
