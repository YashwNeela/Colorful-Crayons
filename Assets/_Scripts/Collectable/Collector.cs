using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Colorful_Crayons
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
            Gamemanager.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
           Invoke(nameof(EnableCollider),2);
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
       protected virtual void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;
            
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

        protected virtual void EnableCollider()
        {
            m_Collider.enabled = true;
        }

        public virtual int GetMaxSnapPoints()
        {
            return snapPoints.Length;
        }
    }
}
