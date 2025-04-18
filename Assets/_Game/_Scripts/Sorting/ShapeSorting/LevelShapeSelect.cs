using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting{
public class LevelShapeSelect : SortingLevel
{
        [SerializeField] private ShapeType m_ShapeType; 
        [SerializeField] private new int m_ScoreRequiredToCompleteTheLevel;

        [SerializeField] List<ShapeSelect> m_ShapeSelectList;

        public List<ShapeSelect> ShapeSelectList => m_ShapeSelectList;

        protected override void Awake()
        {
            m_ShapeSelectList = GetComponentsInChildren<ShapeSelect>().ToList();
        }
        protected override  void OnEnable() 
        {
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnGameRestart += OnGameRestart;
            SortingGameManager.OnGameWin += OnGameWin;
            SortingGameManager.OnLevelCompleteCheck += OnLevelCompleteCheck;
            SubScribeToShapeSelectActions();
            
        }

        protected override void OnDisable()
        {
            
            
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnGameRestart -= OnGameRestart;
            SortingGameManager.OnGameWin -= OnGameWin;
            SortingGameManager.OnLevelCompleteCheck -= OnLevelCompleteCheck;

            UnSubScribeToShapeSelectActions();
        }

        

        private void SubScribeToShapeSelectActions()
        {
            for(int i = 0;i<m_ShapeSelectList.Count;i++)
            {
                m_ShapeSelectList[i].OnShapeSelected += OnShapeSelected;
                m_ShapeSelectList[i].OnShapeDeselected += OnShapeDeselected;
            }
        }

       

        private void UnSubScribeToShapeSelectActions()
        {
             for(int i = 0;i<m_ShapeSelectList.Count;i++)
            {
                m_ShapeSelectList[i].OnShapeSelected -= OnShapeSelected;
                m_ShapeSelectList[i].OnShapeDeselected -= OnShapeDeselected;
            }
        }

        private void OnShapeSelected(ShapeType shapeType)
        {
            if(m_ShapeType.HasFlag(shapeType))
                base.m_CurrentScore++;
            else
                base.m_CurrentScore += 0.3f;


          //  OnItemCollected();
        }

         private void OnShapeDeselected(ShapeType shapeType)
        {
            
            if(m_ShapeType.HasFlag(shapeType))
                base.m_CurrentScore--;
            else
                base.m_CurrentScore -= 0.3f;


            
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
}
}
