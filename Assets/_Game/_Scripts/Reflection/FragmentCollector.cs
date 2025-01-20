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
        public ParticleSystem m_LightEnterPS;


        [SerializeField] Fragment[] m_FragmentsArray;

        Fragment[] FragmentArray => m_FragmentsArray;

         public SpriteRenderer targetSprite; // Assign the SpriteRenderer in the Inspector
        public float lerpDuration = 2.0f;

        public ReflectionLevel m_ReflectionLevel;
        protected virtual void Awake()
        {
             targetSprite.color = new Color(0.5f, 0.5f, 0.5f);
             m_ReflectionLevel = GetComponentInParent<ReflectionLevel>();

          //  m_FragmentsArray = transform.GetComponentsInChildren<Fragment>();
        }

        

        public void OnSunlightEnter()
        {
            transform.DOComplete();
            
            Debug.Log("Sunlight enter Collector");
            if(IsLevelCompleted())
            {
                Debug.Log("Level Completed");
                transform.DOScale(transform.localScale * 1.1f, 0.5f);
             targetSprite.color = new Color(0.5f, 0.5f, 0.5f); // Gray (128, 128, 128)

            // Use DOTween to transition to white
            targetSprite.DOColor(Color.white, lerpDuration) // Tween the color to white
                .SetEase(Ease.Linear)
                .OnComplete(()=> 
                {
                    m_LightEnterPS.Play();
                    m_ReflectionLevel.LevelPass();
                }); // Smooth linear transition


            }
        }

        public void OnSunlightExit()
        {
            transform.DOComplete();
            transform.DOScale(transform.localScale / 1.1f, 0.5f);
            m_LightEnterPS.Stop();

             targetSprite.DOKill();

             targetSprite.color = new Color(0.5f, 0.5f, 0.5f); 
            m_ReflectionLevel.LevelFail();

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
