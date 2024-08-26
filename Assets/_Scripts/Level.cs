using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;


namespace TMKOC.Colorful_Crayons
{
    public class Level : MonoBehaviour
    {
        private int m_ScoreRequiredToCompleteTheLevel;

        public int m_CurrentScore;

        private Collector[] m_Collectors;

        void Awake()
        {
            m_Collectors =GetComponentsInChildren<Collector>();
            SetScoreRequiredToCompleteTheLevel();
        }

        void OnEnable()
        {
            SubscribeToOnItemCollectedAction();
            Gamemanager.OnGameRestart += OnGameRestart;
        }

        private void OnGameRestart()
        {
            m_CurrentScore = 0;
        }

        void OnDisable()
        {
            UnSubscribeToOnItemCollectedAction();
            Gamemanager.OnGameRestart -= OnGameRestart;

        }
        
        private void SetScoreRequiredToCompleteTheLevel()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_ScoreRequiredToCompleteTheLevel += m_Collectors[i].GetMaxSnapPoints();
            }
        }

        private void SubscribeToOnItemCollectedAction()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_Collectors[i].OnItemCollectedAction += OnItemCollected;
            }
        }

        private void UnSubscribeToOnItemCollectedAction()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_Collectors[i].OnItemCollectedAction -= OnItemCollected;
            }
        }

        private void OnItemCollected()
        {
            m_CurrentScore++;
            if(m_CurrentScore >= m_ScoreRequiredToCompleteTheLevel){
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameWin();
            }
        }
    }
}
