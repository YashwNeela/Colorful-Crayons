using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using MoreMountains.Tools;
using TMKOC.PlantLifeCycle;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class SunlightSource : MonoBehaviour
    {
        public LineRenderer m_SunlightLine;

        public int m_Reflections;
        public float m_MaxRayDistance;
        public LayerMask m_LayerDetection;
        public float m_RotationSpeed;
        public Material m_Material;

        private HashSet<Mirror> currentMirrors = new HashSet<Mirror>();


        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
        }

        // Update is called once per frame
void Update()
{
    m_SunlightLine.positionCount = 1;
    m_SunlightLine.SetPosition(0, transform.position);

    RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.right, m_MaxRayDistance, m_LayerDetection);

    Ray2D ray = new Ray2D(transform.position, transform.right);

    HashSet<Mirror> newMirrors = new HashSet<Mirror>();
    Vector2 currentRayOrigin = transform.position;
    Vector2 currentRayDirection = transform.right;
    bool isMirror = false;
    Vector2 mirrorHitPoint = Vector2.zero;
    Vector2 mirrorHitNormal = Vector2.zero;

    for (int i = 0; i < m_Reflections; i++)
    {
        m_SunlightLine.positionCount += 1;

        // Perform raycast
        hitInfo = Physics2D.Raycast(currentRayOrigin, currentRayDirection, m_MaxRayDistance, m_LayerDetection);

        if (hitInfo.collider != null)
        {
            m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1, hitInfo.point);

            // Check if the hit object is a mirror
            if (hitInfo.transform.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
            {
                if(tag.m_Tag == ReflectionTagsEnum.Mirror){
                
                newMirrors.Add(tag.GetComponentInParent<Mirror>()); // Add to new mirrors list

                // Store mirror hit details for later use
                mirrorHitPoint = hitInfo.point;
                mirrorHitNormal = hitInfo.normal;
                isMirror = true;

                // Reflect the ray
                currentRayOrigin = hitInfo.point - currentRayDirection * -0.1f;
                currentRayDirection = Vector2.Reflect(currentRayDirection, hitInfo.normal);
                }
            }
            else
            {
                break; // Stop processing if it's not a mirror
            }
            
        }
        else
        {
            // Handle cases where the ray doesn't hit any collider
            if (isMirror)
            {
                m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                    mirrorHitPoint + Vector2.Reflect(mirrorHitPoint, mirrorHitNormal) * m_MaxRayDistance);
                break;
            }
            else
            {
                m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                    currentRayOrigin + currentRayDirection * m_MaxRayDistance);
                break;
            }
        }
    }

    // Call OnMirrorTriggerEnter for new mirrors
    foreach (var mirror in newMirrors)
    {
        if (!currentMirrors.Contains(mirror))
        {
            OnMirrorTriggerEnter(mirror);
        }
    }

    // Call OnMirrorTriggerExit for mirrors no longer hit
    foreach (var mirror in currentMirrors)
    {
        if (!newMirrors.Contains(mirror))
        {
            OnMirrorTriggerExit(mirror);
        }
    }

    // Update current mirrors
    currentMirrors = newMirrors;
}

        public virtual void OnMirrorTriggerEnter(Mirror mirror) 
        {

            Debug.Log("Enter Mirror " + mirror.name);
         }

        public virtual void OnMirrorTriggerExit(Mirror mirror) 
        {

            Debug.Log("Exit Mirror " + mirror.name);

         }

        public virtual void OnFragmentTriggerEnter(Fragment fragment) { }

        public virtual void OnFragmentTriggerExit(Fragment fragment) { }
    }
}
