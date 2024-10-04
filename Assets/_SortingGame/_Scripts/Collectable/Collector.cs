using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Sorting
{
    public abstract class Collector : MonoBehaviour
    {
        [SerializeField] protected SnapPoint[] snapPoints;

        [SerializeField] protected int collectedItems;

        public UnityAction OnItemCollectedAction;

        protected Component m_Collider;


        protected virtual void OnEnable()
        {
            if (m_Collider is Collider)
            {
                // It's a 3D Collider
                ((Collider)m_Collider).enabled = false;
            }
            else if (m_Collider is Collider2D)
            {
                // It's a 2D Collider
                ((Collider2D)m_Collider).enabled = false;
            }

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

            if (m_Collider == null)
            {
                m_Collider = GetComponent<Collider2D>();
            }
        }

        public virtual void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
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
                    PlacedCorrectly?.Invoke();
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
            if (m_Collider is Collider)
            {
                // It's a 3D Collider
                ((Collider)m_Collider).enabled = true;
            }
            else if (m_Collider is Collider2D)
            {
                // It's a 2D Collider
                ((Collider2D)m_Collider).enabled = true;
            }

        }

        protected virtual void DisableCollider()
        {
            if (m_Collider is Collider)
            {
                // It's a 3D Collider
                ((Collider)m_Collider).enabled = false;
            }
            else if (m_Collider is Collider2D)
            {
                // It's a 2D Collider
                ((Collider2D)m_Collider).enabled = false;
            }


        }

        public virtual int GetMaxSnapPoints()
        {
            return snapPoints.Length;
        }
    }
}
