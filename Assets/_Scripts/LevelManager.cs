using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TMKOC.Colorful_Crayons
{
    public class LevelManager : Singleton<LevelManager>
    {
        public TextMeshProUGUI m_LevelText;
        [SerializeField] private List<GameObject> levels; // Array to hold all levels

        public int MaxLevels => levels.Count;
        private int currentLevelIndex = 0;

        public int CurrentLevelIndex => currentLevelIndex;

    

        private Gamemanager gameManager;

        void Start()
        {
            gameManager = FindObjectOfType<Gamemanager>();

        
        }

        public void CompleteLevel()
        {
            // Deactivate the current level
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
                gameManager.GameOver(); // Assuming you have a method in GameManager to handle this
            }
        }

        public void RestartLevel()
        {
            // Reset the current level (useful if the player fails and needs to retry)
            levels[currentLevelIndex].gameObject.SetActive(false);
           levels[currentLevelIndex].gameObject.SetActive(true);
        }

        public void LoadLevel(int levelIndex)
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
            }
        }

        
    }
}
