using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionGameManager : GameManager
    {


        public override void Start()
        {
            FirstTimeGameStart();
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
            StartIntroCutScene();
        }

        public override void StartIntroCutScene()
        {
            base.StartIntroCutScene();
            CinematicCutSceneManager.Instance.StartCutScene(1, () =>
            {
                Debug.Log("Cut scene ended");
                StartCoroutine(StaticCoroutine.Co_GenericCoroutine(5, () =>
                {
                    GameStart(levelNumber);
                }));
            });

        }

    }


}
