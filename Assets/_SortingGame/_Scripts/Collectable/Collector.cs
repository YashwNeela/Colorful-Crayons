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

        public UnityAction OnItemRemovedAction;

        protected Component m_Collider;

        protected Level m_Level;

        [SerializeField] protected bool m_ShouldIncludeScore = true;


        protected virtual void OnEnable()
        {
            // if (m_Collider is Collider)
            // {
            //     // It's a 3D Collider
            //     ((Collider)m_Collider).enabled = false;
            // }
            // else if (m_Collider is Collider2D)
            // {
            //     // It's a 2D Collider
            //     ((Collider2D)m_Collider).enabled = false;
            // }

            // DisableCollider();
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

        protected virtual void OnDragStartedStaticAction(Transform transform)
        {
            //EnableCollider();
        }

        protected virtual void OnDraggingStaticAction(Transform transform)
        {
            // EnableCollider();
        }
        protected virtual void OnDragEndStaticAction(Transform transform)
        {
            // DisableCollider();
            // Invoke(nameof(DisableCollider),1f);
        }

        private void OnGameStart()
        {
            Invoke(nameof(FetchCollectedItemCount), 0.5f);

        }

        private void FetchCollectedItemCount()
        {
            collectedItems = 0;
            for (int i = 0; i < snapPoints.Length; i++)
            {
                if (snapPoints[i].HasValidCollectible())
                {
                    collectedItems++;
                    m_Level.m_CurrentScore++;
                }
            }

        }



        protected virtual void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Level = GetComponentInParent<Level>();
            if (m_Collider == null)
            {
                m_Collider = GetComponent<Collider2D>();
            }
        }

        public virtual void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            Debug.Log("Collector Box Snap");

            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent

                    snapPoint.IsOccupied = true;
                    collectible.SetSnapPoint(snapPoint);
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                }
            }
        }

        public virtual void SnapCollectibleToCollector(Collectible collectible, SnapPoint snapPoint, Action PlacedCorrectly)
        {
            // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
            collectible.transform.parent = snapPoint.transform; // Change parent first
            collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
            collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent

            snapPoint.IsOccupied = true;
            collectible.SetSnapPoint(snapPoint);
            OnItemCollected(snapPoint);
            PlacedCorrectly?.Invoke();
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

        protected virtual void OnItemRemoved()
        {
            collectedItems--;
            OnItemRemovedAction?.Invoke();
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

        public virtual int GetScore()
        {
            if(m_ShouldIncludeScore)
                return GetMaxSnapPoints();
            return
                0;

        }

        public virtual bool IsSlotAvailable()
        {
            for (int i = 0; i < snapPoints.Length; i++)
            {
                if (!snapPoints[i].IsOccupied)
                    return true;
            }
            return false;
        }

        public virtual SnapPoint GetValidSnapPoint(Collectible collectible)
        {
            return null;
        }
    }
}
