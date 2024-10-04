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
            Gamemanager.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            draggable.m_CanDrag = true;
        }

        private void OnGameRestart()
        {
            m_ObjectReseter.ResetObject();
            Reset();
        }

        protected virtual void OnDisable()
        {
            Gamemanager.OnGameRestart -= OnGameRestart;
            Gamemanager.OnGameStart -= OnGameStart;

        }


        protected virtual void Awake()
        {
            // Subscribe to the OnDragEnd event
            draggable = GetComponent<Draggable>();
            m_ObjectReseter = GetComponent<ObjectReseter>();
            if (draggable != null)
            {
                draggable.OnDragStarted += HandleDragStart;
                draggable.OnDragEnd += HandleDragEnd;
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            HandleCollectorOnTriggerEnter(other);
        }

        protected virtual void HandleCollectorOnTriggerEnter(Component collider)
        {
            if(collider is Collider)
                m_CurrentCollector = ((Collider)collider).GetComponent<Collector>();
            else if(collider is Collider2D)
                m_CurrentCollector = ((Collider2D)collider).GetComponent<Collector>();

            Debug.Log("Trigger Entered");
            if(m_CurrentCollector != null && !m_IsPlaced && draggable.IsDragging)
                m_CurrentCollector.OnCollectibleEntered(this);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            

        }

        protected virtual void HandleCollectorOnTriggerStay(Component collider)
        {

        }

        protected virtual void OnTriggerExit(Collider other)
        {
            HandleCollectorOnTriggerExit(other);
        }
        protected virtual void HandleCollectorOnTriggerExit(Component collider)
        {
            Debug.Log("Trigger Exited");
            if(collider is Collider)
                m_CurrentCollector = ((Collider)collider).GetComponent<Collector>();
            else if(collider is Collider2D)
                m_CurrentCollector = ((Collider2D)collider).GetComponent<Collector>();
            
            if(m_CurrentCollector != null && !m_IsPlaced && draggable.IsDragging)
                m_CurrentCollector.OnCollectibleExited(this);

        }

        protected virtual void HandleDragEnd()
        {
            if (m_ValidCollector != null)
            {
                m_ValidCollector.SnapCollectibleToCollector(this,()=> OnPlacedCorrectly());
            }
            else if(m_IsTryingToPlaceWrong)
                PlaceInCorrectly(m_CurrentCollector);
            
            m_IsTryingToPlaceWrong = false;
                
        }

        protected virtual void HandleDragStart()
        {
            
        }

        protected virtual void OnPlacedCorrectly()
        {
            m_IsPlaced = true;
            draggable.m_CanDrag = false;
        }

        protected virtual void PlaceInCorrectly(Collector collector)
        {
            if(collector != null)
                collector.OnWrongItemTriedToCollect();
            draggable.m_CanDrag = true;
            
           
        }

        protected void Reset()
        {
            m_IsPlaced = false;
            draggable.m_CanDrag = true;
        }
    }
}
