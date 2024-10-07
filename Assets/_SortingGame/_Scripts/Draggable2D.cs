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

            offset = gameObject.transform.position - GetMouseWorldPosition();
            HandleStartDragging();
        }

        private void OnMouseDrag()
        {
            if (!m_isDragging && !m_CanDrag)
                return;

            // Move the object to the mouse position while maintaining the offset
            transform.position = GetMouseWorldPosition() + offset;

            OnDragging?.Invoke();
            OnDraggingStaticAction?.Invoke(transform);
        }

        /// <summary>
        /// OnMouseUp is called when the user has released the mouse button.
        /// </summary>
        void OnMouseUp()
        {
            if(!m_isDragging)
                return;

             HandleStopDragging();
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
