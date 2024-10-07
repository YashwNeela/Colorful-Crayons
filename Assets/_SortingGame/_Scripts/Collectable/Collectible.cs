using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Sirenix.OdinInspector;


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

        [SerializeField] protected bool m_HasCustomSnapPoint;

        [ShowIf("m_HasCustomSnapPoint")]
        [SerializeField] protected SnapPoint m_CurrentSnapPoint;

        [SerializeField] protected bool m_HasCustomDefaulParent;

        [ShowIf("m_HasCustomDefaulParent")]
        [SerializeField] protected Transform m_DefaulParent;

        protected Component m_Collider;

        protected virtual void OnEnable()
        {
            Gamemanager.OnGameRestart += OnGameRestart;
            Gamemanager.OnGameStart += OnGameStart;
            Draggable.OnDragStartedStaticAction += OnDragStartedStaticAction;
            Draggable.OnDraggingStaticAction += OnDraggingStaticAction;
            Draggable.OnDragEndStaticAction += OnDragEndStaticAction;
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
            Draggable.OnDragStartedStaticAction += OnDragStartedStaticAction;
            Draggable.OnDraggingStaticAction += OnDraggingStaticAction;
            Draggable.OnDragEndStaticAction += OnDragEndStaticAction;

        }

        protected virtual void OnDragEndStaticAction(Transform transform)
        {
            
        }

        protected virtual void OnDraggingStaticAction(Transform transform)
        {
            
        }

        protected virtual void OnDragStartedStaticAction(Transform transform)
        {
            
        }

        protected virtual void Awake()
        {
            if(!m_HasCustomDefaulParent)
                m_DefaulParent = transform.parent;

            // Subscribe to the OnDragEnd event
            draggable = GetComponent<Draggable>();
            m_ObjectReseter = GetComponent<ObjectReseter>();
            if (draggable != null)
            {
                draggable.OnDragStarted += HandleDragStart;
                draggable.OnDragEnd += HandleDragEnd;
            }

            m_Collider = GetComponent<Collider>();

            if(m_Collider == null)
            m_Collider = GetComponent<Collider2D>();
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
            HandleCollectorOnTriggerStay(other);

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

        public virtual void SetSnapPoint(SnapPoint snapPoint)
        {
            m_CurrentSnapPoint = snapPoint;
        }

        public virtual void RemoveFromSnapPoint()
        {
            transform.parent = m_DefaulParent;
            if(m_CurrentSnapPoint != null)
                m_CurrentSnapPoint.ResetSnapPoint();

            Reset();
        }
    }
}
