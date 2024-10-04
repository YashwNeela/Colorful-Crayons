using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class Draggable2D : Draggable
    {

        private Vector3 offset;

        public override void Update()
        {
            
        }

        private void OnMouseDown()
        {
            if(!m_CanDrag)
                return;

            if (Gamemanager.Instance.CurrentGameState != GameState.Playing)
                    return;

                if (m_Collider is Collider)
                    ((Collider)m_Collider).isTrigger = true;
                else if (m_Collider is Collider2D)
                    ((Collider2D)m_Collider).isTrigger = true;
            
            // Calculate the offset between the object's position and the mouse position in world space
            offset = gameObject.transform.position - GetMouseWorldPosition();
            m_isDragging = true;
            m_hasDragStarted = true;


            OnDragStarted?.Invoke();
            OnDragStartedStaticAction?.Invoke();
        }

        private void OnMouseDrag()
        {
            if (!m_isDragging && !m_CanDrag)
                return;

            // Move the object to the mouse position while maintaining the offset
            transform.position = GetMouseWorldPosition() + offset;

            OnDragging?.Invoke();
            OnDraggingStaticAction?.Invoke();
        }

        /// <summary>
        /// OnMouseUp is called when the user has released the mouse button.
        /// </summary>
        void OnMouseUp()
        {
            if(!m_isDragging)
                return;

             m_isDragging = false;
            m_hasDragStarted = false;

            if (m_Collider is Collider)
                ((Collider)m_Collider).isTrigger = false;
            else if (m_Collider is Collider2D)
                ((Collider2D)m_Collider).isTrigger = false;

            OnDragEnd?.Invoke();
            OnDragEndStaticAction?.Invoke();
        }

        private Vector3 GetMouseWorldPosition()
        {
            // Get the mouse position in screen coordinates and convert it to world space
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z; // Keep the z-coordinate consistent
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

    }
}
