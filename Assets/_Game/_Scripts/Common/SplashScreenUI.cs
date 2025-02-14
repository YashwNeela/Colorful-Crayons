using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC
{
    public abstract class  SplashScreenUI : SerializedSingleton<SplashScreenUI>
    {
        public GameObject m_Container;

        public virtual void  OnEnable()
        {

        }

        public virtual void OnDisable()
        {
            
        }

        public virtual void EnableSplashScreen()
        {
            m_Container.SetActive(true);
        }

        public virtual void DisableSplashScreen()
        {
            m_Container.SetActive(false);
        }
    }
}
