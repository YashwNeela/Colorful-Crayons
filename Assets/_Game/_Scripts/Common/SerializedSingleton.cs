using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace TMKOC
{
    public class SerializedSingleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
    {
        [SerializeField] protected bool m_ShouldDestroyOnLoad = false;
        private static T _instance;

        private static bool m_applicationIsQuitting = false;

        public void OnDestroy() {
            m_applicationIsQuitting = true;
        }

        public static T Instance
        {
            get
            {
                if (m_applicationIsQuitting) 
                {
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        
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
                _instance = this as T;
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
