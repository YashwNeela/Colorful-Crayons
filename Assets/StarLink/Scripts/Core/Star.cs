using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using UnityEngine;

namespace TMKOC.StarLink
{
    public class Star : MonoBehaviour
    {
        [Header("Orbit Properties")]
        public float orbitRadius = 2f;
        public float orbitSpeed = 90f; // degrees per second

        [Header("Visuals")]
        public SpriteRenderer glowRenderer;
        
        [SerializeField]private bool isActiveStar = false;
        [SerializeField]private bool isTargetStar = false;

        [SerializeField] private ParticleSystem vfxOnActive;

        private float pulseTimer = 0f;

        private void Update()
        {
            if (isTargetStar && glowRenderer != null)
            {
                // Simple pulsing effect for the target star
                pulseTimer += Time.deltaTime * 5f;
                float alpha = (Mathf.Sin(pulseTimer) + 1f) * 0.5f; // 0 to 1
                Color c = glowRenderer.color;
                c.a = Mathf.Lerp(0.3f, 1f, alpha);
                glowRenderer.color = c;
            }
        }

        public void SetAsActive(bool active)
        {
            isActiveStar = active;
            glowRenderer.gameObject.SetActive(true);
            // Optionally change visual state for active star
            if (glowRenderer != null)
            {
                Color c = glowRenderer.color;
                c.a = active ? 1f : 0f;
                Debug.Log("zero alpha" + gameObject.name);
                glowRenderer.color = c;
            }

            if(active)
            {
                vfxOnActive.Play();
            }
        }

        public void SetAsTarget(bool target)
        {
            isTargetStar = target;
            if (!target && glowRenderer != null)
            {
            //    Reset alpha if no longer target
                Color c = glowRenderer.color;
                c.a = 0f;
                Debug.Log("zero alpha" + gameObject.name);

                glowRenderer.color = c;
            }
        }

        public bool IsTarget()
        {
               // vfxOnActive.Stop();

            return isTargetStar;
        }

        public bool IsActive()
        {
            return isActiveStar;
        }
    }
}
