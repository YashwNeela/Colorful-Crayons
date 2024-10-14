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
        
        public float m_CurrentScore;

        private Collector[] m_Collectors;

        private Collectible[] m_Collectibles;

        private Draggable[] m_Draggables;

        [SerializeField]
        public UnityEvent onGameStart;

        [SerializeField]
        public UnityEvent onGameRestart;

        [SerializeField]
        public UnityEvent onLevelCompleteCheck;

        [SerializeField]
        public UnityEvent onGameWin;



       

        public string m_Tip;

        protected virtual void Awake()
        {
            m_Collectors =GetComponentsInChildren<Collector>();
            m_Collectibles = GetComponentsInChildren<Collectible>();
            m_Draggables = GetComponentsInChildren<Draggable>();
             Invoke(nameof(SetScoreRequiredToCompleteTheLevel),2);
        }

        protected virtual void OnEnable()
        {
           
            SubscribeToOnItemCollectedAction();
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameRestart += OnGameRestart;
            Gamemanager.OnGameWin += OnGameWin;
            Gamemanager.OnLevelCompleteCheck += OnLevelCompleteCheck;
        }

        protected virtual void OnGameWin()
        {
            onGameWin?.Invoke();
        }

        protected virtual void OnGameStart()
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
            Gamemanager.OnGameWin -= OnGameWin;

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
            onLevelCompleteCheck?.Invoke();
            if(m_CurrentScore == m_ScoreRequiredToCompleteTheLevel){
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameWin();
            }else
            {
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameLoose();
            }
        }

        public virtual void OnLevelAnimationFinished()
        {
            for(int i = 0;i< m_Draggables.Length;i++)
            {
                m_Draggables[i].m_CanDrag = true;
            }
        }
    }
}
