
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Mirror : Interactable
    {
        public bool m_IsFacingLeft;
        public GameObject m_MirrorObject;
        private Vector2 initialMousePosition;

        MirrorSlider m_MirrorSlider;

        [SerializeField] ParticleSystem m_SunlightEnterPE;

        private bool m_IsActive;

        public bool IsActive => m_IsActive;

        ReflectionLevel m_ReflectionLevel;


        void Awake()
        {
            m_ReflectionLevel = GetComponentInParent<ReflectionLevel>();
        }
        void OnEnable()
        {
            m_ReflectionLevel.OnReflectionLevelLoaded += OnReflectionLevelLoaded;

            m_ReflectionLevel.OnRelectionLevelUnloaded += OnRelectionLevelUnloaded;
        }

        void OnDisable()
        {
            m_ReflectionLevel.OnReflectionLevelLoaded -= OnReflectionLevelLoaded;

            m_ReflectionLevel.OnRelectionLevelUnloaded -= OnRelectionLevelUnloaded;
        }

        private void OnRelectionLevelUnloaded()
        {
            m_IsActive = false;
        }

        private void OnReflectionLevelLoaded()
        {
            m_IsActive = true;
        }

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
                if(!TutorialManager.Instance.IsTutorialActive)
                    CinemachineCameraManager.Instance.ChangeCamera(m_ReflectionLevel.LevelCamera);
                // Get the current value of the slider
                float sliderValue = m_MirrorSlider.m_Slider.value;

                float zRotation;

                // Map the slider value to the desired angle range
                if (m_IsFacingLeft)
                {
                    zRotation = Mathf.Lerp(-45f, 45f, sliderValue);
                }
                else
                {
                    zRotation = Mathf.Lerp(45f, -45f, sliderValue);
                }

                // Apply the calculated rotation to the mirror object on the Z-axis
                if (m_MirrorObject != null)
                {
                    if (m_IsFacingLeft)
                    {
                        // Facing right: Normal rotation
                        m_MirrorObject.transform.rotation = Quaternion.Euler(0, 0, zRotation);
                    }
                    else
                    {
                        // Facing left: Flip on Y and apply Z rotation
                        m_MirrorObject.transform.rotation = Quaternion.Euler(0, 180, -zRotation);
                    }
                }
            }

        }

        protected override void OnPlayerExitZone()
        {
            base.OnPlayerExitZone();
            if(!TutorialManager.Instance.IsTutorialActive)
                    CinemachineCameraManager.Instance.RestoreToDefaultCamera();
            m_MirrorSlider.DisableSlider();
            m_MirrorSlider.OnSliderValueChangedAction -= OnSliderValueChangedAction;

            m_MirrorSlider = null;

        }

        public virtual void OnSunglightEnter()
        {
            m_SunlightEnterPE.Play();
            ReflectionAudioManager.Instance.PlayAudio(
               (ReflectionAudioManager.Instance as ReflectionAudioManager).ReflectionAudioSfx.m_LightReflects[Random.Range(0,(ReflectionAudioManager.Instance as ReflectionAudioManager).ReflectionAudioSfx.m_LightReflects.Count)],
               ReflectionAudioManager.Instance.ExtraAudioSource);
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
