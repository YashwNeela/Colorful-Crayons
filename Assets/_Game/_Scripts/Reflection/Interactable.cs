using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class Interactable : MonoBehaviour
    {   
        public Canvas m_UICanvas;
        public float m_PlayerCheckRadius;
        protected bool m_IsPlayerWithin;
        protected bool m_PreviousPlayerState = false;


        protected virtual void Start()
        {
            m_UICanvas.gameObject.SetActive(false);
        }
        protected virtual void Update()
        {
            Vector2 detectionCenter = transform.position;

            // Get all colliders within the circle
            Collider2D[] colliders = Physics2D.OverlapCircleAll(detectionCenter, m_PlayerCheckRadius);

            m_IsPlayerWithin = false; // Reset the flag

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent<ReflectionTags>(out ReflectionTags tag)) // Check for the ReflectionTags component
                {
                    if (tag.m_Tag == ReflectionTagsEnum.Player)
                    {
                        m_IsPlayerWithin = true;
                        break; // Exit loop as soon as the player is detected
                    }
                }
            }

            // Detect if the player's state has changed
            if (m_IsPlayerWithin != m_PreviousPlayerState)
            {
                if (m_IsPlayerWithin)
                {
                    OnPlayerEnterZone();
                }
                else
                {
                    OnPlayerExitZone();
                }

                // Update the previous state
                m_PreviousPlayerState = m_IsPlayerWithin;
            }
        }


        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
        //     {
        //         if (tag.m_Tag == ReflectionTagsEnum.Player)
        //         {
        //             OnPlayerEnterZone();
        //         }
        //     }
        // }

        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (other.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
        //     {
        //         if (tag.m_Tag == ReflectionTagsEnum.Player)
        //         {
        //             OnPlayerExitZone();
        //         }
        //     }
        // }

        protected virtual void OnPlayerEnterZone()
        {
            m_IsPlayerWithin = true;
            m_UICanvas.gameObject.SetActive(true);
            Debug.Log("Player entered the zone!");
        }

        // Called when the player exits the zone
        protected virtual void OnPlayerExitZone()
        {
            m_IsPlayerWithin = false;
            m_UICanvas.gameObject.SetActive(false);
            
            Debug.Log("Player exited the zone!");
        }

        protected virtual /// <summary>
        /// Callback to draw gizmos only if the object is selected.
        /// </summary>
         /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position,m_PlayerCheckRadius);
            
        }
        

    }
}