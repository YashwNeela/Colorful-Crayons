using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;


namespace TMKOC.Reflection
{
    public enum FragmentType
    {
        Light, Water, Earth, Mixed
    }
    public class Fragment : MonoBehaviour
    {
        public FragmentType m_FragmentType;

        public ParticleSystem m_LightEnterPS;
        public SpriteRenderer targetSprite; // Assign the SpriteRenderer in the Inspector
        
        public float lerpDuration = 2.0f;

        public Vector3 m_OriginalScale;

        protected bool m_IsCollected;

        public bool IsCollected => m_IsCollected;

        protected bool m_IsScaling;

        Coroutine ref_CO;

        protected virtual void Awake()
        {
            m_OriginalScale = transform.lossyScale;
            m_IsScaling = false;
            // targetSprite.color = new Color(0.5f, 0.5f, 0.5f); 

        }

        protected virtual void OnPlayerTriggerEnter() { }

        protected virtual void OnPlayerExitTrigger() { }
        

        public virtual void OnSunlightTriggerEnter()
        {

            transform.DOComplete();
            ReflectionAudioManager.Instance.PlayAudio(
               (ReflectionAudioManager.Instance as ReflectionAudioManager).ReflectionAudioSfx.m_FragmentCollected,
               ReflectionAudioManager.Instance.ExtraAudioSource,false,true);

            transform.DOScale(transform.localScale * 1.1f, 0.5f)
            .OnComplete(()=>
            {
                    targetSprite.transform.DOScale(targetSprite.transform.localScale * 1.1f,0.5f).SetLoops(-1,LoopType.Yoyo);

            });

            // targetSprite.color = new Color(0.5f, 0.5f, 0.5f); // Gray (128, 128, 128)
            // Use DOTween to transition to white
            targetSprite
                .DOColor(Color.white, lerpDuration) // Tween the color to white
                .SetEase(Ease.Linear)
                .OnComplete(()=> {
                    m_LightEnterPS.Play();
                    }); // Smooth linear transition


            m_IsCollected = true;


        }

        public virtual void OnSunlightTriggerExit()
        {
            transform.DOComplete();

            transform.DOScale(transform.localScale / 1.1f, 0.5f);
            targetSprite.DOKill();
            targetSprite.transform.DOKill();
            //DOTween.Kill(breath);
             //targetSprite.color = new Color(0.5f, 0.5f, 0.5f); 
             m_LightEnterPS.Stop();

            m_IsCollected = false;



        }

        



    }
}
