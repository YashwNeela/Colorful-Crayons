using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Mirror : Interactable
    {
        public GameObject m_MirrorObject;
        private Vector2 initialMousePosition;

        MirrorSlider m_MirrorSlider;

        [SerializeField] ParticleSystem m_SunlightEnterPE;



        protected override void OnPlayerEnterZone()
        {
            base.OnPlayerEnterZone();
            m_MirrorSlider = MirrorSlider.Instance;
            m_MirrorSlider.EnableSlider();
            m_MirrorSlider.OnSliderValueChangedAction += OnSliderValueChangedAction;
        }

        private void OnSliderValueChangedAction(float value)
        {
             if (m_MirrorSlider != null)
            {
                // Get the current value of the slider
                float sliderValue = m_MirrorSlider.m_Slider.value;

                // Map the slider value to the desired angle range, e.g., -45 to 45
                float zRotation = Mathf.Lerp(-45f, 45f, sliderValue);

                // Apply the calculated rotation to the mirror object on the Z-axis
                if (m_MirrorObject != null)
                {
                    m_MirrorObject.transform.rotation = Quaternion.Euler(0, 180, zRotation);
                }
            }
        }

        protected override void OnPlayerExitZone()
        {
            base.OnPlayerExitZone();
            m_MirrorSlider.DisableSlider();
            m_MirrorSlider.OnSliderValueChangedAction -= OnSliderValueChangedAction;

            m_MirrorSlider = null;
            
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
