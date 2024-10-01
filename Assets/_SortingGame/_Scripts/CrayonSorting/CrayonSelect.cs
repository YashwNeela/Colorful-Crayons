using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class CrayonSelect : Crayon
    {
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

        }

        protected override void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
            
            Gamemanager.OnGameRestart += OnGameRestart;
            
        }

        private void OnGameStart()
        {
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
            Gamemanager.OnGameStart -= OnGameStart;

            Gamemanager.OnGameRestart -= OnGameRestart;
            
        }

        private void OnMouseDown() 
        {
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

        
    }
}
