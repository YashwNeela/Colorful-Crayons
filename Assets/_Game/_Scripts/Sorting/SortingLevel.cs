using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


namespace TMKOC.Sorting
{
    public class SortingLevel : Level
    {
        protected int m_ScoreRequiredToCompleteTheLevel;

        public float m_CurrentScore;

        [SerializeField] int m_LevelTimer;

        public int LevelTimer => m_LevelTimer;

        protected Collector[] m_Collectors;

        public Collector[] Collectors => m_Collectors;

        protected Collectible[] m_Collectibles;

        public Collectible[] Collectibles => m_Collectibles;

        protected Draggable[] m_Draggables;

        public Draggable[] Draggables => m_Draggables;

        [SerializeField]
        public UnityEvent onGameStart;

        [SerializeField]
        public UnityEvent onGameRestart;

        [SerializeField]
        public UnityEvent onGameWin;


        protected override void Awake()
        {
            base.Awake();
            m_Collectors = GetComponentsInChildren<Collector>();
            m_Collectibles = GetComponentsInChildren<Collectible>();
            m_Draggables = GetComponentsInChildren<Draggable>();
            Invoke(nameof(SetScoreRequiredToCompleteTheLevel), 2);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SubscribeToOnItemCollectedAction();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnSubscribeToOnItemCollectedAction();
        }

        protected override void OnGameWin()
        {
            onGameWin?.Invoke();
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();
            onGameStart?.Invoke();
        }

        protected override void OnGameRestart()
        {
            base.OnGameRestart();
            m_CurrentScore = 0;
            onGameRestart?.Invoke();
        }



        protected virtual void SetScoreRequiredToCompleteTheLevel()
        {
            for (int i = 0; i < m_Collectors.Length; i++)
            {
                m_ScoreRequiredToCompleteTheLevel += m_Collectors[i].GetScore();
            }
        }

        protected virtual void SubscribeToOnItemCollectedAction()
        {
            for (int i = 0; i < m_Collectors.Length; i++)
            {
                m_Collectors[i].OnItemCollectedAction += OnItemCollected;
                m_Collectors[i].OnItemRemovedAction += OnItemRemoved;
            }
        }

        protected virtual void UnSubscribeToOnItemCollectedAction()
        {
            for (int i = 0; i < m_Collectors.Length; i++)
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

        protected override void OnLevelCompleteCheck()
        {

            base.OnLevelCompleteCheck();
            if (m_CurrentScore == m_ScoreRequiredToCompleteTheLevel)
            {
                SortingGameManager.Instance.GameOver();
                SortingGameManager.Instance.GameWin();
            }
            else
            {
                (SortingGameManager.Instance as SortingGameManager).WrongAnswer();

                SortingGameManager.Instance.GameOver();
                SortingGameManager.Instance.GameLoose();
            }
        }


        public virtual void OnLevelAnimationFinished()
        {
            for (int i = 0; i < m_Draggables.Length; i++)
            {
                m_Draggables[i].m_CanDrag = true;
            }
        }
    }
}
