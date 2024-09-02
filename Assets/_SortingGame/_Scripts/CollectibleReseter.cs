using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class CollectibleReseter : MonoBehaviour
    {
        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Collectible>(out Collectible c))
            {
                if (c.TryGetComponent<ObjectReseter>(out ObjectReseter reseter))
                {
                    reseter.ResetObject();
                }

            }
        }
    }

   
}
