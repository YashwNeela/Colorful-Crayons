using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Sorting
{
    public abstract class Collector : MonoBehaviour
    {
        [SerializeField] protected SnapPoint[] snapPoints;

        [SerializeField] protected int collectedItems;

        public UnityAction OnItemCollectedAction;

        protected Collider m_Collider;


        protected virtual void OnEnable()
        {
            m_Collider.enabled = false;
            DisableCollider();
            Gamemanager.OnGameStart += OnGameStart;
            Draggable.OnDragStartedStaticAction += OnDragStartedStaticAction;
            Draggable.OnDraggingStaticAction += OnDraggingStaticAction;
            Draggable.OnDragEndStaticAction += OnDragEndStaticAction;
        }

        


        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;
            Draggable.OnDragStartedStaticAction -= OnDragStartedStaticAction;
            Draggable.OnDraggingStaticAction -= OnDraggingStaticAction;
            Draggable.OnDragEndStaticAction -= OnDragEndStaticAction;


        }

        private void OnDragStartedStaticAction()
        {
            EnableCollider();
        }

        private void OnDraggingStaticAction()
        {
           // EnableCollider();
        }
        private void OnDragEndStaticAction()
        {
            DisableCollider();
           // Invoke(nameof(DisableCollider),1f);
        }

        private void OnGameStart()
        {
            
        }

        protected virtual void Awake()
        {
            m_Collider = GetComponent<Collider>();
        }

        public virtual void SnapCollectibleToCollector(Collectible collectible)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    break;
                }
            }
        }

        public virtual void OnCollectibleEntered(Collectible collectible)
        {

        }

        public virtual void OnCollectibleExited(Collectible collectible)
        {

        }

        protected virtual void OnItemCollected(SnapPoint snapPoint)
        {
            collectedItems++;
            OnItemCollectedAction?.Invoke();
        }

        public virtual void OnWrongItemTriedToCollect()
        {

        }

        protected virtual void EnableCollider()
        {
            m_Collider.enabled = true;
        }

        protected virtual void DisableCollider()
        {
            m_Collider.enabled = false;

        }

        public virtual int GetMaxSnapPoints()
        {
            return snapPoints.Length;
        }
    }
}
