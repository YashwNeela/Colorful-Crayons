using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMPro;
using UnityEngine;

namespace TMKOC
{
    public class LevelManager : Singleton<LevelManager>
    {
        public TextMeshProUGUI m_LevelText;
        public TextMeshProUGUI m_TipText;

        [SerializeField] private List<GameObject> levels; // Array to hold all levels

        public int MaxLevels => levels.Count;
        private int currentLevelIndex = 0;

        public int CurrentLevelIndex => currentLevelIndex;

        private GameManager m_GameManager;

        protected virtual void Start()
        {
            m_GameManager = FindAnyObjectByType<GameManager>();
        }

        public virtual SortingLevel GetCurrentLevel()
        {
            return levels[CurrentLevelIndex].GetComponent<SortingLevel>();
        }

        public virtual void CompleteLevel()
        {
            levels[currentLevelIndex].gameObject.SetActive(false);


            // Increment level index
            currentLevelIndex++;

            // Check if there are more levels
            if (currentLevelIndex < levels.Count)
            {
                // Activate the next level
                levels[currentLevelIndex].gameObject.SetActive(true);
            }
            else
            {
                // No more levels, handle game completion
                m_GameManager.GameOver(); // Assuming you have a method in GameManager to handle this
            }
        }

        public virtual void RestartLevel()
        {
            // Reset the current level (useful if the player fails and needs to retry)
            levels[currentLevelIndex].gameObject.SetActive(false);
            levels[currentLevelIndex].gameObject.SetActive(true);
        }

        public virtual void LoadLevel(int levelIndex)
        {
             if (levelIndex >= 0 && levelIndex < levels.Count)
            {
                // Deactivate all levels
                foreach (var level in levels)
                {
                    level.gameObject.SetActive(false);
                }

                // Activate the requested level
                currentLevelIndex = levelIndex;
                levels[currentLevelIndex].gameObject.SetActive(true);

                m_LevelText.text =  (currentLevelIndex + 1).ToString() + "/" + MaxLevels.ToString();
                m_TipText.text = levels[currentLevelIndex].GetComponent<Level>().m_Tip;
            }
        }




    }
}
