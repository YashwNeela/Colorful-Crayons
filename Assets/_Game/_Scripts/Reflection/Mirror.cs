using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Mirror : Interactable
    {
        public GameObject m_MirrorObject;
        private Vector2 initialMousePosition;

        MirrorJoyStick m_MirrorJoystick;

        protected override void OnPlayerEnterZone()
        {
            base.OnPlayerEnterZone();
            m_MirrorJoystick = MirrorJoyStick.Instance;
            m_MirrorJoystick.EnableJoystick();
        }

        protected override void OnPlayerExitZone()
        {
            base.OnPlayerExitZone();
            m_MirrorJoystick.DisableJoystick();
            m_MirrorJoystick = null;
        }

        protected override void Update()
        {
            base.Update();
            if(m_MirrorJoystick != null){
                
            Vector2 direction =  m_MirrorJoystick.Direction;
            if (direction != Vector2.zero)
                transform.right = direction;
            }
        }

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

        public void RotateObject()
        {
            // Rotate the object by 90 degrees around the z-axis
          m_MirrorObject.transform.Rotate(0f, 0f, 90f);
        } 

    }
}
