using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetKits.ParticleImage;
using UnityEngine.Events;



namespace TMKOC.Sorting
{

    public class ConfettiUI : Singleton<ConfettiUI>
    {
        [SerializeField] private ParticleImage m_ParticleImage;

        public UnityAction OnConfettiParticleStartAction;

        public UnityAction OnConfettiLastParticleFinishedAction;

        public void OnParticleStart()
        {
            OnConfettiParticleStartAction?.Invoke();
        }

        public void OnLastParticleFinished()
        {
        
            OnConfettiLastParticleFinishedAction?.Invoke();
        }

        public void PlayParticle()
        {
            m_ParticleImage.Play();
        }
    }
}
