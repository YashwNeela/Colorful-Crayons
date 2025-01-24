using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace TMKOC
{
    public class CinemachineCameraManager : SerializedSingleton<CinemachineCameraManager>
    {
        private CinemachineBrain m_CinemachineBrain;

        public CinemachineBrain CinemachineBrain => m_CinemachineBrain;

        private CinemachineVirtualCameraBase m_DefaultCinemachineCamera;

        public CinemachineVirtualCameraBase DefaultCinemachineCamera => m_DefaultCinemachineCamera;

        void Start()
        {
            m_CinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            m_DefaultCinemachineCamera = m_CinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
        }

        public void ChangeCamera(CinemachineVirtualCameraBase cameraToActivate, Action transitionCompleted = null)
        {
            StartCoroutine(MonitorCameraTransition(cameraToActivate, transitionCompleted));

            CinemachineVirtualCameraBase currentCamera = m_CinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCameraBase;

            currentCamera.Priority = -1;
            cameraToActivate.Priority = 1;

        }

        private System.Collections.IEnumerator MonitorCameraTransition(
                CinemachineVirtualCameraBase targetCamera,
                Action transitionCompleted)
        {
                yield return new WaitForSeconds(1); // Wait for the next frame
            
            // Wait for the target camera to become active in the CinemachineBrain
            while (Camera.main.GetComponent<CinemachineBrain>().IsBlending)
            {
                yield return null; // Wait for the next frame
            }

            // Optional: Add a small delay to ensure the transition is visually complete
            yield return new WaitForSeconds(0.1f);

            // Invoke the callback when the transition is complete
            transitionCompleted?.Invoke();

            Debug.Log("Cineachine Camera Transtion Completed");
        }


        public void RestoreToDefaultCamera(Action transitionCompleted = null)
        {
            CinemachineVirtualCameraBase currentCamera = m_CinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCameraBase;
            currentCamera.Priority = -1;

            m_DefaultCinemachineCamera.Priority = 1;

            StartCoroutine(MonitorCameraTransition(m_DefaultCinemachineCamera, transitionCompleted));

        }
    }
}
