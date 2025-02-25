using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{
    [System.Serializable]
    public struct LivesData
    {
        public bool m_IsEnable;

        public Image m_HeartImage;
    }
    public class SortingLivesUI : MonoBehaviour
    {
        public LivesData[] m_HeartImageData;

        public void OnEnable()
        {
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnWrongAnswerAction += OnWrongAnswerAction;
        }

        private void OnWrongAnswerAction()
        {
            for (int i = 0; i < m_HeartImageData.Length; i++)
            {
                if (m_HeartImageData[i].m_IsEnable == true)
                {
                    m_HeartImageData[i].m_IsEnable = false;

                    m_HeartImageData[i].m_HeartImage.color = Color.black;
                    return;
                }

            }
        }

        private void OnGameStart()
        {
            for (int i = 0; i < m_HeartImageData.Length; i++)
            {
                m_HeartImageData[i].m_IsEnable = true;
                m_HeartImageData[i].m_HeartImage.color = Color.white;

            }
        }

        public void OnDisable()
        {
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnWrongAnswerAction -= OnWrongAnswerAction;

        }
    }
}
