using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMKOC.Colorful_Crayons
{
    public abstract class Collectible : MonoBehaviour
    {
        protected Collector currentCollector;

        protected bool m_IsTryingToPlaceWrong;

        protected ObjectReseter m_ObjectReseter;

        protected virtual void OnEnable()
        {
            Gamemanager.OnGameRestart += OnGameRestart;
        }

        private void OnGameRestart()
        {
            m_ObjectReseter.ResetObject();
        }

        protected virtual void OnDisable()
        {
            Gamemanager.OnGameRestart -= OnGameRestart;

        }


        protected virtual void Awake()
        {
            // Subscribe to the OnDragEnd event
            var draggable = GetComponent<Draggable>();
            m_ObjectReseter = GetComponent<ObjectReseter>();
            if (draggable != null)
            {
                draggable.OnDragEnd += HandleDragEnd;
            }
        }

        protected virtual void HandleDragEnd()
        {
            if (currentCollector != null)
            {
                currentCollector.SnapCollectibleToCollector(this);
                OnPlacedCorrectly();
            }
            else if(m_IsTryingToPlaceWrong)
                OnPlacedInCorrectly();
            
            m_IsTryingToPlaceWrong = false;
                
        }

        protected abstract void OnPlacedCorrectly();

        protected abstract void OnPlacedInCorrectly();

        protected void Reset()
        {

        }
    }
}
