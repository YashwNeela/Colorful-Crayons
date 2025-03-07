using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Reflection
{
    public class ReflectionSplashScreenUI : SplashScreenUI
    {
        public Animator m_Animator;

        public Button m_PlayButton;

        public override void OnEnable()
        {
            base.OnEnable();
            m_PlayButton.onClick.AddListener(()=>
            {
                GameManager.Instance.FirstTimeGameStart();
                 ReflectionAudioManager.Instance.PlayAudio(
                (ReflectionAudioManager.Instance as ReflectionAudioManager).ReflectionAudioSfx.m_ButtonClick,
                ReflectionAudioManager.Instance.ExtraAudioSource);
                DisableSplashScreen();
            });
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void EnableSplashScreen()
        {
            base.EnableSplashScreen();
            m_Animator.SetTrigger("BeamAnim");
        }

        public void OnBeamAnimationFinished()
        {
            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(2,()=>
            {
                m_PlayButton.gameObject.SetActive(true);
                m_PlayButton.GetComponent<DOTweenAnimation>().DOPlay();
            }));
        
        }

        public override void DisableSplashScreen()
        {
            base.DisableSplashScreen();
            m_PlayButton.GetComponent<DOTweenAnimation>().DORewind();

        }

    }
}
