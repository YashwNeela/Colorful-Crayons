using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{
public class ReflectionLevelManager : LevelManager
{
    public override void LoadLevel(int levelIndex)
        {
             if (levelIndex >= 0 && levelIndex < levels.Count)
            {
                // Deactivate all levels
                foreach (var level in levels)
                {
                    levels[currentLevelIndex].GetComponent<Level>().OnLevelUnloaded();

                    //level.gameObject.SetActive(false);

                }

                Debug.Log($"Current Level: {levelIndex}");

                // Activate the requested level
                 currentLevelIndex = levelIndex;
                levels[currentLevelIndex].gameObject.SetActive(true);
                levels[currentLevelIndex].GetComponent<Level>().OnLevelLoaded();

                HelperGameCategoryDataSaver.LevelCompleted(currentLevelIndex); // Save Current Level

                m_LevelText.text =  (currentLevelIndex + 1).ToString() + "/" + MaxLevels.ToString();
                m_TipText.text = levels[currentLevelIndex].GetComponent<Level>().m_Tip;
            }
        }
}
}
