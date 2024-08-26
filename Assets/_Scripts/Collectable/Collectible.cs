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
            Collector collector = other.GetComponent<Collector>();
            if(collector != null && !m_IsPlaced && draggable.IsDragging)
                collector.OnCollectibleEntered(this);
        }

        protected virtual void OnTriggerExit(Collider other)
        {

            Collector collector = other.GetComponent<Collector>();
            if(collector != null && !m_IsPlaced)
                collector.OnCollectibleExited(this);
        }

        protected virtual void HandleDragEnd()
        {
            if (currentCollector != null)
            {
                currentCollector.SnapCollectibleToCollector(this);
                OnPlacedCorrectly();
            }
            else if(m_IsTryingToPlaceWrong)
                PlaceInCorrectly();
            
            m_IsTryingToPlaceWrong = false;
                
        }

        protected virtual void OnPlacedCorrectly()
        {
            m_IsPlaced = true;
        }

        protected virtual void PlaceInCorrectly(){}

        protected void Reset()
        {
            m_IsPlaced = false;
        }
    }
}
