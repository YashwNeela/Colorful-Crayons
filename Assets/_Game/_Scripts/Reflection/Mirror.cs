using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Mirror : Interactable
    {
        public GameObject m_MirrorObject;
        private Vector2 initialMousePosition;

        void OnMouseDown()
        {
            if (m_IsPlayerWithin)
            {
                initialMousePosition = Input.mousePosition; // Store the initial mouse position
            }
        }

        void OnMouseDrag()
        {
            if (m_IsPlayerWithin)
            {
                Vector2 currentMousePosition = Input.mousePosition;
                RotateObject(initialMousePosition, currentMousePosition);
                initialMousePosition = currentMousePosition; // Update for the next frame
            }
        }

        void RotateObject(Vector2 initialPosition, Vector2 currentPosition)
        {
            Vector2 direction = currentPosition - initialPosition;
            float rotationAngle = direction.magnitude; // Use the distance moved to determine the rotation
            m_MirrorObject.transform.Rotate(0f, 0f, rotationAngle);
        }

    }
}
