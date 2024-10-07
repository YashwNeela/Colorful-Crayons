using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


namespace TMKOC.Sorting
{
    public class Level : MonoBehaviour
    {
        protected int m_ScoreRequiredToCompleteTheLevel;
        
        public int m_CurrentScore;

        private Collector[] m_Collectors;

        [SerializeField]
        public UnityEvent onGameStart;

        [SerializeField]
        public UnityEvent onGameRestart;

        public string m_Tip;

        protected virtual void Awake()
        {
            m_Collectors =GetComponentsInChildren<Collector>();
             Invoke(nameof(SetScoreRequiredToCompleteTheLevel),2);
        }

        protected virtual void OnEnable()
        {
           
            SubscribeToOnItemCollectedAction();
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameRestart += OnGameRestart;
            Gamemanager.OnLevelCompleteCheck += OnLevelCompleteCheck;
        }

        private void OnGameStart()
        {
            onGameStart?.Invoke();
        }

        protected virtual void OnGameRestart()
        {
            m_CurrentScore = 0;
            onGameRestart?.Invoke();
        }

        protected virtual void OnDisable()
        {
            UnSubscribeToOnItemCollectedAction();
            Gamemanager.OnGameStart -= OnGameStart;

            Gamemanager.OnGameRestart -= OnGameRestart;
            Gamemanager.OnLevelCompleteCheck -= OnLevelCompleteCheck;


        }
        
        protected virtual void SetScoreRequiredToCompleteTheLevel()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_ScoreRequiredToCompleteTheLevel += m_Collectors[i].GetMaxSnapPoints();
            }
        }

        protected virtual void SubscribeToOnItemCollectedAction()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_Collectors[i].OnItemCollectedAction += OnItemCollected;
                m_Collectors[i].OnItemRemovedAction += OnItemRemoved;
            }
        }

        protected virtual void UnSubscribeToOnItemCollectedAction()
        {
            for(int i = 0 ;i<m_Collectors.Length;i++)
            {
                m_Collectors[i].OnItemCollectedAction -= OnItemCollected;
                m_Collectors[i].OnItemRemovedAction -= OnItemRemoved;

            }
        }

        protected virtual void OnItemCollected()
        {
            m_CurrentScore++;
            // if(m_CurrentScore >= m_ScoreRequiredToCompleteTheLevel){
            //     Gamemanager.Instance.GameOver();
            //     Gamemanager.Instance.GameWin();
            // }
        }

        protected virtual void OnItemRemoved()
        {
            m_CurrentScore--;

        }

        protected virtual void OnLevelCompleteCheck()
        {
            if(m_CurrentScore == m_ScoreRequiredToCompleteTheLevel){
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameWin();
            }else
            {
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameLoose();
            }
        }
    }
}
