using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionLevel : Level
    {
        public Transform m_CameraPosition;

        public GameObject m_DarkEnvironment;

        public GameObject m_LightEnvironment;

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
                    m_CameraPosition = t.transform;
                    break;
                }
            }
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            
            Camera.main.transform.position = new Vector3(m_CameraPosition.position.x, m_CameraPosition.position.y,
          Camera.main.transform.position.z);
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
