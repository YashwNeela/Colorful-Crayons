using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.FruitSorting2D;
using UnityEngine;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class CrayonSelect : Crayon
    {
         private bool m_CanSelect;
        [SerializeField] private SpriteRenderer m_CrayonColorSprite;

        public Action<CrayonColor> OnCrayonSelected;
        public Action<CrayonColor> OnCrayonDeselected;

       [SerializeField] private bool m_IsSelected;

        [SerializeField] DOTweenAnimation m_InitialDotweenAnimation;
        private Sequence m_SelectedSequence;

        protected override void Awake()
        {
             m_ObjectReseter = GetComponent<ObjectReseter>();

            m_SelectedSequence = DOTween.Sequence();
            m_SelectedSequence.Append(transform.DOScale(new Vector3(transform.lossyScale.x * 1.2f,transform.lossyScale.y * 1.2f,transform.lossyScale.z * 1.2f),0.5f));
            m_SelectedSequence.SetAutoKill(false);
            m_SelectedSequence.Pause();
            SetCrayonColor(m_CrayonColor);

        }

        protected override void SetCrayonColor(CrayonColor crayonColor)
        {
            m_CrayonColorSprite.sprite = CrayonSpriteManager.Instance.CrayonSpriteSO.CrayonSprites[crayonColor];
        }

        protected override void OnEnable()
        {
            SortingGameManager.OnGameStart += OnGameStart;
            
            SortingGameManager.OnGameRestart += OnGameRestart;
            
        }

        private void OnGameStart()
        {
            m_CanSelect = false;
            m_IsSelected = false;
            m_InitialDotweenAnimation.DOPlayForward();
        }

        private void OnGameRestart()
        {

            m_SelectedSequence.Rewind();
            m_InitialDotweenAnimation.DORewind();
            m_ObjectReseter.ResetObject();
        }

        protected override void OnDisable()
        {
            SortingGameManager.OnGameStart -= OnGameStart;

            SortingGameManager.OnGameRestart -= OnGameRestart;
            
        }

        private void OnMouseDown() 
        {
            if (SortingGameManager.Instance.CurrentGameState != GameState.Playing)
                return;

            if(!m_CanSelect)
                return;
        
            m_IsSelected = m_IsSelected?false:true;

            if(m_IsSelected)
                CrayonSelected();
            else
                CrayonDeselected();
        }

        private void CrayonSelected()
        {
            m_SelectedSequence.PlayForward();
            OnCrayonSelected?.Invoke(m_CrayonColor);

            LevelCrayonSelect crayonSelectionLevel = SortingLevelManager.Instance.GetCurrentLevel() as LevelCrayonSelect;

            if (crayonSelectionLevel.m_CurrentScore == crayonSelectionLevel.ScoreRequiredToCompleteTheLevel())
                Invoke(nameof(CheckForLevelComplete), 1);
        }

        public void CheckForLevelComplete()
        {

            SortingGameManager.Instance.LevelCompleteCheck();
        }
        private void CrayonDeselected()
        {
            m_SelectedSequence.PlayBackwards();
            OnCrayonDeselected?.Invoke(m_CrayonColor);
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

        public void OnInitialDotweenAnimationFinished()
        {
            m_CanSelect = true;
        }

        
    }
}
