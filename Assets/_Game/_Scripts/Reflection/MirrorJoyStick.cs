using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class MirrorJoyStick : FixedJoystick
    {
         [SerializeField] protected bool m_ShouldDestroyOnLoad = false;
        private static MirrorJoyStick _instance;

        public static MirrorJoyStick Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MirrorJoyStick>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<MirrorJoyStick>();
                        singletonObject.name = typeof(MirrorJoyStick).ToString() + " (Singleton)";
                        
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as MirrorJoyStick;
                if(!m_ShouldDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
