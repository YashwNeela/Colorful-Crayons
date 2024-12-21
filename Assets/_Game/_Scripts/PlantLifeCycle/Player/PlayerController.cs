using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.PlantLifeCycle
{
    public class PlayerController : MonoBehaviour
    {
        public void PlayGroundDiggingParicleEffect()
        {
            ParticleEffectManager.Instance.PlayParticleEffect(0,transform.position, Vector3.one,null);
        }
    }
}
