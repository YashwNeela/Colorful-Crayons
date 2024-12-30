using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Interactable : MonoBehaviour
    {
        public float m_PlayerCheckRadius;
       protected bool m_IsPlayerWithin;
        protected bool m_PreviousPlayerState = false;

      

     

         private void OnTriggerEnter2D(Collider2D other) {
            if(other.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
            {
                if(tag.m_Tag == ReflectionTagsEnum.Player)
                {
                    OnPlayerEnterZone();
                }
            }
        }

         private void OnTriggerExit2D(Collider2D other) {
            if(other.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
            {
                if(tag.m_Tag == ReflectionTagsEnum.Player)
                {
                    OnPlayerExitZone();
                }
            }
        }

        protected virtual void OnPlayerEnterZone()
        {
             m_IsPlayerWithin = true;
 
            Debug.Log("Player entered the zone!");
        }

        // Called when the player exits the zone
        protected virtual void OnPlayerExitZone()
        {
                        m_IsPlayerWithin = false;

            Debug.Log("Player exited the zone!");
        }

    }
}