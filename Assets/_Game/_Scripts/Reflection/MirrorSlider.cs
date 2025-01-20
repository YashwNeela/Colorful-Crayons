using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Slider UI component

namespace TMKOC.Reflection
{
    [System.Serializable]
    public class MirrorSlider : MonoBehaviour
    {
        [SerializeField] protected bool m_ShouldDestroyOnLoad = false;
        private static MirrorSlider _instance;

        [SerializeField] public GameObject m_Container; // Container for the slider
        [SerializeField] public Slider m_Slider;        // Reference to the Slider component

        public static MirrorSlider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MirrorSlider>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<MirrorSlider>();
                        singletonObject.name = typeof(MirrorSlider).ToString() + " (Singleton)";
                        
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
                _instance = this as MirrorSlider;
                if (!m_ShouldDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void Start()
        {
            // Ensure slider is initially disabled
            DisableSlider();

            // Optionally, set up the slider value range or event listener
            if (m_Slider != null)
            {
                m_Slider.minValue = 0;
                m_Slider.maxValue = 1;
                m_Slider.value = 0.5f;

                // Example listener for slider value changes
                m_Slider.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        public void EnableSlider()
        {
            if (m_Container != null)
                m_Container.SetActive(true);
        }

        public void DisableSlider()
        {
            if (m_Container != null)
                m_Container.SetActive(false);
        }

        private void OnSliderValueChanged(float value)
        {
            Debug.Log($"Slider Value Changed: {value}");
        }
    }
}
