using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        if (crayonColor.HasFlag(CrayonColor.CrayonRed) || crayonColor.HasFlag(CrayonColor.SketchpenRed))
        {
            m_CrayonColorSprite.color = Color.red;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonYellow) || crayonColor.HasFlag(CrayonColor.SketchpenYellow))
        {
            m_CrayonColorSprite.color = Color.yellow;


        }
        if (crayonColor.HasFlag(CrayonColor.CrayonGreen) || crayonColor.HasFlag(CrayonColor.SketchpenGreen))
        {
            m_CrayonColorSprite.color = Color.green;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonBlue) || crayonColor.HasFlag(CrayonColor.SketchpenBlue))
        {
            m_CrayonColorSprite.color = Color.blue;

        }

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
            if(SortingGameManager.Instance.CurrentGameState != GameState.Playing)
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
