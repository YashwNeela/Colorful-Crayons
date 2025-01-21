using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMKOC.PlantLifeCycle;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Platform : MonoBehaviour
    {
        public FragmentCollector  m_FragmentCollector;
        public GameObject m_MoveAblePlatform;

        public Transform m_StartPos, m_EndPos;

        void OnEnable()
        {
            GameManager.OnGameStart += OnGameStart;
           

        }

        private void OnGameStart()
        {
            m_MoveAblePlatform.transform.localPosition = m_StartPos.position;
        }

        public void OnFragmentUnCollected()
        {
           MoveToPoint(m_StartPos.position);
        }

        public void OnFragmentCollected()
        {
           MoveToPoint(m_EndPos.position);

        }
        

        void OnDisable()
        {
            GameManager.OnGameStart -= OnGameStart;
           
        }

        public void MoveToPoint(Vector3 pos)
        {
            m_MoveAblePlatform.transform.DOMove(pos,3);
        }
    }
}