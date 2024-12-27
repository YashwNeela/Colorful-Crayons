using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace TMKOC.PlantLifeCycle
{
    [System.Serializable]
    public enum PlayerState
    {
        Idle,
        Walking,
        Dancing,
        WaitingToDig,
        Digging
    }
    public class PlayerController : MonoBehaviour
    {
        public PlayerState m_CurrentPlayerState;
        [SerializeField]
        public UnityEvent OnPlayerReadyToDig;

        Animator m_Animator;

        public Transform m_DigSoilPSParent;

        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void SetPlayerState(PlayerState state)
        {
            m_CurrentPlayerState = state;

        }
        public void PlayDigSoilAnimation()
        {

            m_Animator.SetTrigger("dig");
        }

        public void PlayerReadyToDig()
        {
            OnPlayerReadyToDig?.Invoke();
        }

        public void PlayGroundDiggingParicleEffect()
        {
            ParticleEffectManager.Instance.PlayParticleEffect(0,m_DigSoilPSParent.position, Vector3.one,null);
        }
    }
}
