using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class HUDUI : MonoBehaviour
    {
        public Transform m_TipText;
        private bool m_IsShowingTip;
        public void OnBackButtonClicked()
        {
            Gamemanager.Instance.GoBackToPlayschool();
        }

        public void LevelCompleteCheck()
        {
            Gamemanager.Instance.LevelCompleteCheck();
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
            Invoke(nameof(CloseTip), 3f);
        }

        public void CloseTip()
        {
            m_TipText.DOLocalMoveX(445f, .25f);
            m_IsShowingTip = false;

        }
    }
}
