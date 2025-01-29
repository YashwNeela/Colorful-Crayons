using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{
public class ReflectionGameManager : GameManager
{
    public override void FirstTimeGameStart()
        {
            m_CatergoryDataManager = new GameCategoryDataManager(GAMEID,PlayerPrefs.GetString("currentGameName"));
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

            

         StartCoroutine(StaticCoroutine.Co_GenericCoroutine(5, () =>
            {
                GameStart(levelNumber);

            }));


        }
        
    }
}
