using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace TMKOC.Sorting
{
    public class StarCollectorParticleImage : MonoBehaviour
    {
        [SerializeField] private ParticleImage m_ParticleImage;
        public UnityAction OnParticleStartAction;

        public UnityAction OnLastParticleFinishedAction;
        
        public void OnParticleStart()
        {
            OnParticleStartAction?.Invoke();
        }

        public void OnLastParticleFinished()
        {
            OnLastParticleFinishedAction?.Invoke();
        }

        public void PlayParticle()
        {
            m_ParticleImage.Play();
        }

        public void SetEmitter(Transform emitterTransform)
        {
            m_ParticleImage.emitterConstraintTransform = emitterTransform;
        }

        public void SetAttractor(Transform attractorTarget)
        {
            m_ParticleImage.attractorTarget = attractorTarget;
        }
    }
}
