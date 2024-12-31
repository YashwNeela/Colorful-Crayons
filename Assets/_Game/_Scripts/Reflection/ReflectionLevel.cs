using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionLevel : Level
    {
        public Transform m_CameraPosition;

        protected override void Awake()
        {
            base.Awake();
            FetchCameraTransform();
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
    }
}
