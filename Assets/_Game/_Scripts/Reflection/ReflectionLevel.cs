using System;
using System.Collections;
using System.Collections.Generic;
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
