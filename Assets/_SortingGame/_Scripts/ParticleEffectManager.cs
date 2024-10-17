using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting{
public class ParticleEffectManager : SerializedSingleton<ParticleEffectManager>
{
    [SerializeField] ParticleSystem[] m_ParticleSystem;

    public void PlayParticleEffect(int index, Vector3 position, Vector3 scale,Transform parent)
    {
       ParticleSystem p = Instantiate(m_ParticleSystem[index]);

       p.transform.localScale = scale;
       p.transform.parent = parent;

       p.transform.localPosition = Vector3.zero;
       p.Play();
    }


}
}
