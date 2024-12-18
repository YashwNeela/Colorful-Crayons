using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitSelectionLevel : Level
    {
        [SerializeField] private new int m_ScoreRequiredToCompleteTheLevel;
        [SerializeField] private FruitType m_FruitType;
        [SerializeField] List<FruitSelect> m_FruitSelectList;

        public static event Action OnGameEnd;


        protected override void Awake()
        {
            m_FruitSelectList = GetComponentsInChildren<FruitSelect>().ToList();
        }

        protected override void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnLevelCompleteCheck += OnLevelCompleteCheck;
            //Gamemanager.OnGameRestart += base.OnGameRestart;
            SubscribeToFruitSelectActions();
        }
        protected override void OnLevelCompleteCheck()
        {
            onLevelCompleteCheck?.Invoke();
            if (m_CurrentScore == m_ScoreRequiredToCompleteTheLevel)
            {
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameWin();
            }
            else
            {
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameLoose();
            }
            OnGameEnd?.Invoke();
        }


        protected override void OnDisable()
        {;
            Gamemanager.OnGameStart -= OnGameStart;
            Gamemanager.OnLevelCompleteCheck -= OnLevelCompleteCheck;
            //Gamemanager.OnGameRestart -= base.OnGameRestart;
            UnsubscribeToFruitSelectActions();
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();
            SetScoreRequiredToCompleteTheLevel();
        }

        protected override void SetScoreRequiredToCompleteTheLevel()
        {
            base.m_CurrentScore = 0;

            base.m_ScoreRequiredToCompleteTheLevel = this.m_ScoreRequiredToCompleteTheLevel;
        }

        private void OnFruitSelected(FruitType fruit)
        {
            if (m_FruitType.HasFlag(fruit))
                base.m_CurrentScore++;
            else
                base.m_CurrentScore += 0.3f;
        }

        private void OnFruitDeselected(FruitType fruit)
        {
            if (m_FruitType.HasFlag(fruit))
                base.m_CurrentScore--;
            else
                base.m_CurrentScore -= 0.3f;
        }

        private void SubscribeToFruitSelectActions()
        {
            for (int i = 0; i < m_FruitSelectList.Count; i++)
            {
                m_FruitSelectList[i].OnFruitSelected += OnFruitSelected;
                m_FruitSelectList[i].OnFruitDeselected += OnFruitDeselected;
            }
        }



        private void UnsubscribeToFruitSelectActions()
        {
            for (int i = 0; i < m_FruitSelectList.Count; i++)
            {
                m_FruitSelectList[i].OnFruitSelected -= OnFruitSelected;
                m_FruitSelectList[i].OnFruitDeselected -= OnFruitDeselected;
            }
        }
    }
}
