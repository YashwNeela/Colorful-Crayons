using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class LevelCrayonSelect : SortingLevel
    {
        [SerializeField] private CrayonColor m_CrayonColor; 
        [SerializeField] private new int m_ScoreRequiredToCompleteTheLevel;

        [SerializeField] List<CrayonSelect> m_CrayonSelectList;

        public List<CrayonSelect> CrayonSelectList => m_CrayonSelectList;

        protected override void Awake()
        {
            m_CrayonSelectList = GetComponentsInChildren<CrayonSelect>().ToList();
        }
        protected override  void OnEnable() {
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnLevelCompleteCheck += OnLevelCompleteCheck;
            SubScribeToCrayonSelectActions();
            
        }

        protected override void OnDisable()
        {
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnLevelCompleteCheck -= OnLevelCompleteCheck;

            UnSubScribeToCrayonSelectActions();
        }

        

        private void SubScribeToCrayonSelectActions()
        {
            for(int i = 0;i<m_CrayonSelectList.Count;i++)
            {
                m_CrayonSelectList[i].OnCrayonSelected += OnCrayonSelected;
                m_CrayonSelectList[i].OnCrayonDeselected += OnCrayonDeselected;
            }
        }

       

        private void UnSubScribeToCrayonSelectActions()
        {
             for(int i = 0;i<m_CrayonSelectList.Count;i++)
            {
                m_CrayonSelectList[i].OnCrayonSelected -= OnCrayonSelected;
                m_CrayonSelectList[i].OnCrayonDeselected -= OnCrayonDeselected;
            }
        }

        private void OnCrayonSelected(CrayonColor crayonColor)
        {
            if(m_CrayonColor.HasFlag(crayonColor))
                base.m_CurrentScore++;
            else
                base.m_CurrentScore += 0.3f;


          //  OnItemCollected();
        }

         private void OnCrayonDeselected(CrayonColor crayonColor)
        {            
            if(m_CrayonColor.HasFlag(crayonColor))
                base.m_CurrentScore--;
            else
                base.m_CurrentScore -= 0.3f;


            
        }

          private void OnGameStart()
        {
            SetScoreRequiredToCompleteTheLevel();
        }

        protected override void SetScoreRequiredToCompleteTheLevel()
        {
            base.m_CurrentScore = 0;

            base.m_ScoreRequiredToCompleteTheLevel = this.m_ScoreRequiredToCompleteTheLevel;
        }
    }
}
