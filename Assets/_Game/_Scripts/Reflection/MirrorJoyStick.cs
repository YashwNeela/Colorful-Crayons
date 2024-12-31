using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    [System.Serializable]
    public class MirrorJoyStick : FixedJoystick
    {
         [SerializeField] protected bool m_ShouldDestroyOnLoad = false;
        private static MirrorJoyStick _instance;

        [SerializeField] public GameObject m_Container;

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

        protected override void Start()
        {
            base.Start();
            DisableJoystick();
        }

        public void EnableJoystick()
        {
            m_Container.SetActive(true);
        }

        public void DisableJoystick()
        {
            m_Container.SetActive(false);
        }
    }
}
