using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting
{
    public class SnapPoint : MonoBehaviour
    {
        public bool IsOccupied = false;

        [SerializeField] private bool m_HasCustomCollectible;

        [ShowIf("m_HasCustomCollectible")]
        [SerializeField] protected Collectible m_CustomCollectible;

        [SerializeField] protected Collectible m_CurrentCollectible;

        public Collectible CurrentCollectible => m_CurrentCollectible;

        void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameRestart += OnGameRestart;
        }

        private void OnGameRestart()
        {
            if(m_HasCustomCollectible)
            {
                m_CurrentCollectible = m_CustomCollectible;
                IsOccupied = true;
            }
            else
            {
                m_CurrentCollectible = null;
                IsOccupied = false;
            }
        }

        private void OnGameStart()
        {
            if(m_HasCustomCollectible)
            {
                m_CurrentCollectible = m_CustomCollectible;
                IsOccupied = true;
 
            }
            else
            {
                m_CurrentCollectible = null;
 
                IsOccupied = false;

            }
        }

        void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;
            Gamemanager.OnGameRestart -= OnGameRestart;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }

        public void ResetSnapPoint()
        {
            IsOccupied= false;
            m_CurrentCollectible = null;
        }

        public virtual void SetCollectible(Collectible collectible)
        {
            m_CurrentCollectible = collectible;
            IsOccupied = true;
        }

        public virtual bool HasValidCollectible(){return false;}

    }
}
