using UnityEngine;
using System;

namespace TMKOC.Colorful_Crayons
{
    public class Draggable : MonoBehaviour
    {
        private Camera m_Camera;
        private Rigidbody m_Rigidbody;
        private bool isDragging = false;
        private float m_ZPosition;

        public event Action OnDragEnd; // Event to notify when dragging ends

        void Awake()
        {
            m_Camera = Camera.main;
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        void OnMouseDown()
        {
            m_ZPosition = transform.position.z;
            if (m_Rigidbody != null)
            {
                m_Rigidbody.isKinematic = true;
            }
            isDragging = true;
        }

        void OnMouseDrag()
        {
            if (isDragging)
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
            isDragging = false;
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
