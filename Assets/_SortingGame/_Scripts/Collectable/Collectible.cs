using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMKOC.Sorting
{
    public abstract class Collectible : MonoBehaviour
    {
        protected Collector m_ValidCollector;

        protected Collector m_CurrentCollector;

        protected bool m_IsTryingToPlaceWrong;

        protected ObjectReseter m_ObjectReseter;

        protected bool m_IsPlaced = false;

        protected Draggable draggable;

        protected virtual void OnEnable()
        {
            Gamemanager.OnGameRestart += OnGameRestart;
        }

        private void OnGameRestart()
        {
            m_ObjectReseter.ResetObject();
            Reset();
        }

        protected virtual void OnDisable()
        {
            Gamemanager.OnGameRestart -= OnGameRestart;

        }


        protected virtual void Awake()
        {
            // Subscribe to the OnDragEnd event
            draggable = GetComponent<Draggable>();
            m_ObjectReseter = GetComponent<ObjectReseter>();
            if (draggable != null)
            {
                draggable.OnDragEnd += HandleDragEnd;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            m_CurrentCollector = other.GetComponent<Collector>();
            if(m_CurrentCollector != null && !m_IsPlaced && draggable.IsDragging)
                m_CurrentCollector.OnCollectibleEntered(this);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            

        }

        protected virtual void OnTriggerExit(Collider other)
        {
            m_CurrentCollector = other.GetComponent<Collector>();
            if(m_CurrentCollector != null && !m_IsPlaced && draggable.IsDragging)
                m_CurrentCollector.OnCollectibleExited(this);
        }

        protected virtual void HandleDragEnd()
        {
            if (m_ValidCollector != null)
            {
                m_ValidCollector.SnapCollectibleToCollector(this);
                OnPlacedCorrectly();
            }
            else if(m_IsTryingToPlaceWrong)
                PlaceInCorrectly(m_CurrentCollector);
            
            m_IsTryingToPlaceWrong = false;
                
        }

        protected virtual void OnPlacedCorrectly()
        {
            m_IsPlaced = true;
        }

        protected virtual void PlaceInCorrectly(Collector collector)
        {
            if(collector != null)
                collector.OnWrongItemTriedToCollect();
           
        }

        protected void Reset()
        {
            m_IsPlaced = false;
        }
    }
}
