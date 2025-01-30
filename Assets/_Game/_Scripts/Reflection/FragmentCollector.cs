using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMKOC.PlantLifeCycle;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Reflection
{
    public class FragmentCollector : MonoBehaviour
    {
        [SerializeField]protected FragmentType m_FragmentType;
        public ParticleSystem m_LightEnterPS;

        public UnityEvent OnFragmentCollected;

        public UnityEvent OnFragmentUnCollected;

        [SerializeField] Fragment[] m_FragmentsArray;

        Fragment[] FragmentArray => m_FragmentsArray;

         public SpriteRenderer targetSprite; // Assign the SpriteRenderer in the Inspector
        public float lerpDuration = 2.0f;

        public ReflectionLevel m_ReflectionLevel;

        public bool m_IsPartOfTutorial;

        Vector3 startScale;

        public bool m_IsSunlightEnter;
        protected virtual void Awake()
        {
             targetSprite.color = new Color(0.5f, 0.5f, 0.5f);
             m_ReflectionLevel = GetComponentInParent<ReflectionLevel>();
             startScale = transform.localScale;

          //  m_FragmentsArray = transform.GetComponentsInChildren<Fragment>();
        }

        

        public void OnSunlightEnter()
        {
            transform.DOComplete();
            m_IsSunlightEnter = true;
            Debug.Log("Sunlight enter Collector");
            if(IsLevelCompleted())
            {
                Debug.Log("Level Completed");
            transform.DOScale(startScale * 1.1f, 0.5f);
            targetSprite.color = new Color(0.5f, 0.5f, 0.5f); // Gray (128, 128, 128)

            // Use DOTween to transition to white
            targetSprite.DOColor(Color.white, lerpDuration) // Tween the color to white
                .SetEase(Ease.Linear)
                .OnComplete(()=> 
                {
                    m_LightEnterPS.Play();
                    m_ReflectionLevel.LevelPass();
                    OnFragmentCollected?.Invoke();
                    if(!m_IsPartOfTutorial)
                    {
                        Transform attractor = null;
                        Sprite starSprite = null;
                    switch(m_FragmentType)
                    {
                        case FragmentType.Light:
                        attractor = GemsUI.Instance.m_LigthGemContainer;
                        starSprite = GemsUI.Instance.m_LightStar;
                        break;
                        case FragmentType.Water:
                        attractor = GemsUI.Instance.m_WaterGemContainer;
                        starSprite = GemsUI.Instance.m_WaterStar;

                        break;
                        case FragmentType.Earth:
                        attractor = GemsUI.Instance.m_EarthGemContainer;
                        starSprite = GemsUI.Instance.m_EarthStar;


                        break;
                    }
                    StarCollectorParticleImage.Instance.PlayCollectorParticle(starSprite,transform,attractor,
                    null,()=>
                    {
                        GemsUI.Instance.OnGemCollected(m_FragmentType);
                    });
                    }

                }); // Smooth linear transition


            }
        }

        public void OnSunlightExit()
        {
            m_IsSunlightEnter = false;
            transform.DOComplete();
            transform.DOScale(startScale / 1.1f, 0.5f);
            m_LightEnterPS.Stop();

             targetSprite.DOKill();

             targetSprite.color = new Color(0.5f, 0.5f, 0.5f); 
            m_ReflectionLevel.LevelFail();

            OnFragmentUnCollected?.Invoke();

            

        }
        
        public bool IsLevelCompleted()
        {
            if(!m_IsSunlightEnter) return false;
            for(int i = 0;i< m_FragmentsArray.Length;i++)
            {
                if(!m_FragmentsArray[i].IsCollected)
                    return false;
            }
            return true;
        }
    }
}
