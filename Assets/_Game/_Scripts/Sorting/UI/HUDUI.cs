using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{
    public class HUDUI : MonoBehaviour
    {
        public Transform m_TipText;
        private bool m_IsShowingTip;

        [SerializeField] private Button m_CheckButton;


        void OnEnable()
        {
           // SortingGameManager.OnWrongAnswerAction += OnWrongAnswerAction;
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnLevelCompleteCheck +=OnLevelCompleteCheck;
        }

        

        private void OnLevelCompleteCheck()
        {
          //  m_CheckButton.interactable = false;
        }

        void OnDisable()
        {
            //SortingGameManager.OnWrongAnswerAction -= OnWrongAnswerAction;
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnLevelCompleteCheck -=OnLevelCompleteCheck;


        }

        private void OnGameStart()
        {
            m_IsShowingTip = false;
            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(3f,()=>
            {
            m_CheckButton.interactable = true;

            }));
            
            
            ShowTip();
        }

       

        public void OnBackButtonClicked()
        {
            SortingGameManager.Instance.GoBackToPlayschool();
        }

        public void LevelCompleteCheck()
        {
            (SortingGameManager.Instance as SortingGameManager).LevelCompleteCheck();
        }

        public void ShowTip()
        {
            if(m_IsShowingTip)
            {
                CloseTip();
                return;
            }
            m_TipText.DOLocalMoveX(0f, .25f);
            m_IsShowingTip = true;
            //Invoke(nameof(CloseTip), 3f);
        }

        public void CloseTip()
        {
            m_TipText.DOLocalMoveX(473f, .25f);
            m_IsShowingTip = false;

        }
    }
}
