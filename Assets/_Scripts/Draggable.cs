using UnityEngine;
using System;

namespace TMKOC.Colorful_Crayons
{
    public class Draggable : MonoBehaviour
    {
        private Camera m_Camera;
        private Rigidbody m_Rigidbody;
        private bool m_isDragging = false;
        private bool m_hasDragStarted = false;
        private float m_ZPosition;

        public bool HasDragStarted => m_hasDragStarted;
        public bool IsDragging => m_isDragging;

        public event Action OnDragEnd; // Event to notify when dragging ends

        public Collider m_Collider;

        // Ground level Y position
        public float GroundLevel = 0.0f;

        void Awake()
        {
            m_Camera = Camera.main;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Collider = GetComponent<Collider>();
        }

        void OnMouseDown()
        {
            if (Gamemanager.Instance.CurrentGameState != GameState.Playing)
                return;

            m_ZPosition = transform.position.z;
            m_Collider.isTrigger = true;
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = true;
            }
            m_isDragging = true;
            m_hasDragStarted = true;
        }

        void OnMouseDrag()
        {
            if (m_isDragging)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = m_Camera.WorldToScreenPoint(transform.position).z;
                Vector3 worldPosition = m_Camera.ScreenToWorldPoint(mousePosition);
                worldPosition.z = m_ZPosition;

                // Ensure the object doesn't go below the ground level
                if (worldPosition.y < GroundLevel)
                {
                    worldPosition.y = GroundLevel;
                }

                transform.position = worldPosition;
            }
        }

        void OnMouseUp()
        {
            m_isDragging = false;
            m_hasDragStarted = false;
            m_Collider.isTrigger = false;


            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = false;
            }

            // Trigger the drag end event
            OnDragEnd?.Invoke();
        }

        public void HandleRigidbodyKinematic(bool value)
        {
            m_Rigidbody.isKinematic = value;
        }
    }
}
