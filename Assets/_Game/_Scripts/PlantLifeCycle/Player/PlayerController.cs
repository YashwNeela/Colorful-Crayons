using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.PlantLifeCycle
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent OnPlayerReadyToDig;

        Animator m_Animator;

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }
        public void PlayDigSoilAnimation()
        {
            m_Animator.SetTrigger("dig");
        }

        public void OnDigSoilAnimationFinished()
        {

        }

        public void PlayerReadyToDig()
        {
            OnPlayerReadyToDig?.Invoke();
        }

        public void PlayDigAnimation()
        {

        }

        public void PlayGroundDiggingParicleEffect()
        {
            ParticleEffectManager.Instance.PlayParticleEffect(0,transform.position, Vector3.one,null);
        }
    }
}
