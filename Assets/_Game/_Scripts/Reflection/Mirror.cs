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

        [SerializeField] ParticleSystem m_SunlightEnterPE;



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

        public virtual void OnSunglightEnter()
        {
            m_SunlightEnterPE.Play();
        }

        public virtual void OnSunlightExit()
        {
            m_SunlightEnterPE.Stop();
        }

        protected override void Update()
        {
            base.Update();
            if (m_MirrorJoystick != null)
            {
                Vector2 direction = m_MirrorJoystick.Direction;
            //    Debug.Log("Direction is " + direction);

                if (direction != Vector2.zero)
                {
                    // Calculate the angle from the direction vector
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    // Clamp the angle to a desired range, e.g., -180 to 180
                    float zRotation = Mathf.Clamp(angle, -45, 45);

                    // Apply the calculated rotation to the mirror object on the Z-axis
                    m_MirrorObject.transform.rotation = Quaternion.Euler(0, 180, zRotation);
                }
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
