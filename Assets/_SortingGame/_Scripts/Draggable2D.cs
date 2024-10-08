using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class Draggable2D : Draggable
    {

        
        protected override void StartDragging()
        {
            // Raycast to detect if the mouse is over this object
            Vector2 mousePos = m_Camera.ScreenToWorldPoint(Input.mousePosition);

            // Perform a 2D raycast at the mouse position
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // Check if the ray hit this object (2D collider)
            if (hit.collider == m_Collider)
            {
                // Store the current Z position
                m_ZPosition = transform.position.z;

                // Call the method to handle dragging logic
                HandleStartDragging();
            }
        }

        protected override void DragObject()
        {
            // Get the current mouse position in screen space
            Vector3 mousePosition = Input.mousePosition;

            // Keep the Z value of the object consistent
            mousePosition.z = m_Camera.WorldToScreenPoint(transform.position).z;

            // Convert the mouse position to world space in 2D
            Vector3 worldPosition = m_Camera.ScreenToWorldPoint(mousePosition);

            // Ensure the object stays at the correct Z position
            worldPosition.z = m_ZPosition;

            // // Clamp the object's position within the screen bounds
            // if (worldPosition.y < GroundLevel)
            // {
            //     worldPosition.y = GroundLevel;
            // }

            // if (worldPosition.x < screenLeft)
            // {
            //     worldPosition.x = screenLeft;
            // }

            // if (worldPosition.x > screenRight)
            // {
            //     worldPosition.x = screenRight;
            // }

            // Set the object's position to the new world position
            transform.position = worldPosition;

            // Invoke any drag-related events
            OnDragging?.Invoke();
            OnDraggingStaticAction?.Invoke(transform);

        }
    }
}
