using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionLevel : Level
    {
        public Transform m_CameraPosition;

        public Transform m_PlayerSpawnPoint;

        public GameObject m_DarkEnvironment;

        public GameObject m_LightEnvironment;

        public Action OnReflectionLevelLoaded;
        public Action OnRelectionLevelUnloaded;

      [SerializeField]  private CinemachineVirtualCameraBase levelCamera;

        public CinemachineVirtualCameraBase LevelCamera => levelCamera;

        [SerializeField] private CinemachineVirtualCameraBase levelIntroCam;
        public CinemachineVirtualCameraBase LevelIntroCam => levelIntroCam;

        protected override void Awake()
        {
            base.Awake();
            FetchCameraTransform();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LevelFail();
        }
        protected override void OnGameStart()
        {
            base.OnGameStart();
            canTriggerLeveIntro = true;
        }

        private void FetchCameraTransform()
        {
            ReflectionTags[] reflectionTags = gameObject.GetComponentsInChildren<ReflectionTags>();
            foreach(ReflectionTags t in reflectionTags)
            {
                if(t.m_Tag == ReflectionTagsEnum.CameraTransform)
                {
                 //   m_CameraPosition = t.transform;
                    break;
                }
            }
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            OnReflectionLevelLoaded?.Invoke();

            
        }
        public bool canTriggerLeveIntro;
        public void TriggerLevelIntro()
        {
            
            if(!TutorialManager.Instance.IsTutorialActive && canTriggerLeveIntro){
                canTriggerLeveIntro = false;
                 
                ControlsUI.Instance.DisableAllControls();
                FindAnyObjectByType<PlayerStateMachine>().HardCodePointUpOnMovementButton();
                CinemachineCameraManager.Instance.ChangeCamera(levelIntroCam,()=>
                {
                    CinemachineSplineDolly dolly = levelIntroCam.GetComponent<CinemachineSplineDolly>();
                    DOTween.To(()=> dolly.SplineSettings.Position, x=>dolly.SplineSettings.Position =x, 1,5f)
                    .OnComplete(()=>
                    {
                        Debug.Log("Musaaa bhai");
                        CinemachineCameraManager.Instance.RestoreToDefaultCamera(()=>
                        {
                            dolly.SplineSettings.Position = 0;
                        });

                        ControlsUI.Instance.EnableAllControls();

                    });
                });
            }
        }
        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
            OnRelectionLevelUnloaded?.Invoke();

        }

        public void LevelPass()
        {
            //m_LightEnvironment.gameObject.SetActive(true);
            //m_DarkEnvironment.gameObject.SetActive(false);

        }

        public void LevelFail()
        {
            
            //m_DarkEnvironment.gameObject.SetActive(true);
            //m_LightEnvironment.gameObject.SetActive(false);
        }
    }
}
