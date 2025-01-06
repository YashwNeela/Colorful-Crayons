using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMKOC.PlantLifeCycle;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class FragmentCollector : MonoBehaviour
    {
        protected FragmentType m_FragmentType;

         Fragment[] m_FragmentsArray;

        Fragment[] FragmentArray => m_FragmentsArray;

        protected virtual void Awake()
        {
            m_FragmentsArray = transform.GetComponentsInChildren<Fragment>();
        }

        public void OnSunlightEnter()
        {
            transform.DOScale(transform.localScale * 1.1f, 0.5f);
            Debug.Log("Sunlight enter Collector");
            if(IsLevelCompleted())
                Debug.Log("Level Completed");
        }

        public void OnSunlightExit()
        {
            transform.DOScale(transform.localScale / 1.1f, 0.5f);
        }
        
        public bool IsLevelCompleted()
        {
            for(int i = 0;i< m_FragmentsArray.Length;i++)
            {
                if(!m_FragmentsArray[i].IsCollected)
                    return false;
            }
            return true;
        }
    }
}
