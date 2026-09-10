using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.FruitSorting2D;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting{
public class ShapeSelect : Collectible
{
       public Action<ShapeType> OnShapeSelected;
        public Action<ShapeType> OnShapeDeselected;

        [SerializeField] private bool m_IsSelected = false;
        [SerializeField] private ShapeType m_ShapeType;

        [SerializeField] private GameObject m_HighLightGameobject;


        protected override void Awake()
        {
            base.Awake();
            

        }
        protected override void OnEnable()
        {
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnGameRestart += OnGameRestart;
        }
        protected override void OnDisable()
        {
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnGameRestart -= OnGameRestart;
        }

        private void OnGameStart()
        {
          m_HighLightGameobject.SetActive(false);

            m_IsSelected = false;

        }

        private void OnGameRestart()
        {
          m_HighLightGameobject.SetActive(false);
            
            m_ObjectReseter.ResetObject();
        }
        private void OnMouseDown()
        {
            if (SortingGameManager.Instance.CurrentGameState != GameState.Playing)
                return;

            m_IsSelected = m_IsSelected ? false : true;

            if (m_IsSelected)
                ShapeSelected();
            else
                ShapeDeselected();
        }

        private void ShapeSelected()
        {
          //  m_SelectedSequence.Play();
          m_HighLightGameobject.SetActive(true);
            transform.DOScale(transform.localScale * 1.1f, 0.25f);
            OnShapeSelected?.Invoke(m_ShapeType);

            LevelShapeSelect fruitSelectionLevel = SortingLevelManager.Instance.GetCurrentLevel() as LevelShapeSelect;

            if (fruitSelectionLevel.m_CurrentScore == fruitSelectionLevel.ScoreRequiredToCompleteTheLevel())
                Invoke(nameof(CheckForLevelComplete), 1);
        }

        public void CheckForLevelComplete()
        {

            SortingGameManager.Instance.LevelCompleteCheck();
        }

        private void ShapeDeselected()
        {
          m_HighLightGameobject.SetActive(false);

         //   m_SelectedSequence.PlayBackwards();
            transform.DOScale(transform.localScale / 1.1f, 0.25f);
            OnShapeDeselected?.Invoke(m_ShapeType);
        }

        protected override void OnTriggerEnter(Collider other)
        {

        }

        protected override void OnTriggerExit(Collider other)
        {

        }

        protected override void OnTriggerStay(Collider other)
        {

        }

}
}