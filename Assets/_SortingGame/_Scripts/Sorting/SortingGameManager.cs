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
  
    public class SortingGameManager : GameManager
    {
        public GameObject m_LevelCheckButton;
        [SerializeField] protected LevelManager m_LevelManager;

      //  protected DataManager dataManager;

     //   public DataManager DataManager => dataManager;

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

        public override void FirstTimeGameStart()
        {
          base.FirstTimeGameStart();
            if (!testLevel){
                levelNumber = m_CatergoryDataManager.GetCompletedLevel;
                if(levelNumber == LevelManager.Instance.MaxLevels)
                    levelNumber = 0;
            }
            else 
                levelNumber = levelNumber;
            // if(!testLevel)
            // dataManager.FetchData(() =>
            //     {
            //         GameStart(dataManager.StudentGameData.data.completedLevel);
            //     });

            GameStart(levelNumber);
                

            m_LevelCheckButton.GetComponentInChildren<Button>().interactable = false;

            Invoke(nameof(EnableLevelCheckButton),2);
            
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
        public override void GameStart(int level)
        {
            base.GameStart(level);
            SetRemainingLives(m_MaxLives);
            SetScore(0);
        }

        public override void GameNotCompleted()
        {
            base.GameNotCompleted();
            ConfettiUI.Instance.PlayParticle();
            Invoke(nameof(PlayLevelCompletedBlast), 2);
        }

        private void PlayLevelCompletedBlast()
        {
            ParticleSystem p = Instantiate(m_LevelCompletedBlast);
            p.transform.position = m_LevelCompleteBlastParent.position + m_LevelCompletedBlastOffset;

            p.Play();
            LoadNextLevel(LevelManager.Instance.CurrentLevelIndex + 1);


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