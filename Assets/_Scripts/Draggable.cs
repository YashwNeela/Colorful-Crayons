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

        public bool HasDragStarted => m_hasDragStarted;

        public bool IsDragging => m_isDragging;
        private float m_ZPosition;

        public event Action OnDragEnd; // Event to notify when dragging ends

        void Awake()
        {
            m_Camera = Camera.main;
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void OnMouseDown()
        {
            if(Gamemanager.Instance.CurrentGameState != GameState.Playing)
                return;
            m_ZPosition = transform.position.z;
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
                transform.position = worldPosition;
            }
        }

        void OnMouseUp()
        {
            m_isDragging = false;
            m_hasDragStarted = false;

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
