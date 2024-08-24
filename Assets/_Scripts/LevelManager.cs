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
        [SerializeField] private GameObject[] levels; // Array to hold all levels
        private int currentLevelIndex = 0;

        private Gamemanager gameManager;

        void Start()
        {
            gameManager = FindObjectOfType<Gamemanager>();

        
        }

        public void CompleteLevel()
        {
            // Deactivate the current level
            levels[currentLevelIndex].SetActive(false);

            // Increment level index
            currentLevelIndex++;

            // Check if there are more levels
            if (currentLevelIndex < levels.Length)
            {
                // Activate the next level
                levels[currentLevelIndex].SetActive(true);
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
            levels[currentLevelIndex].SetActive(false);
            levels[currentLevelIndex].SetActive(true);
        }

        public void LoadLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < levels.Length)
            {
                // Deactivate all levels
                foreach (var level in levels)
                {
                    level.SetActive(false);
                }

                // Activate the requested level
                currentLevelIndex = levelIndex;
                levels[currentLevelIndex].SetActive(true);

                m_LevelText.text = "Level " + (currentLevelIndex + 1).ToString();
            }
        }
    }
}
