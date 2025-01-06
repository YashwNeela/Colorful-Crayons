using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography;
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

        private HashSet<Fragment> currentFragments = new HashSet<Fragment>();

        private HashSet<FragmentCollector> currentragmentCollector = new HashSet<FragmentCollector>();



        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
        }

        // Update is called once per frame
        void Update()
        {
            m_SunlightLine.positionCount = 1;
            m_SunlightLine.SetPosition(0, transform.position);

            Vector2 currentRayOrigin = transform.position;
            Vector2 currentRayDirection = transform.right;

            HashSet<Mirror> newMirrors = new HashSet<Mirror>();
            HashSet<Fragment> newFragments = new HashSet<Fragment>();
            HashSet<FragmentCollector> newFragmentCollector = new HashSet<FragmentCollector>();

            bool isMirror = false;
            Vector2 mirrorHitPoint = Vector2.zero;
            Vector2 mirrorHitNormal = Vector2.zero;

            for (int i = 0; i < m_Reflections; i++)
            {
                m_SunlightLine.positionCount += 1;

                RaycastHit2D hitInfo = Physics2D.Raycast(currentRayOrigin, currentRayDirection, m_MaxRayDistance, m_LayerDetection);

                if (hitInfo.collider != null)
                {
                    m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1, hitInfo.point);

                    // Handle Mirrors
                    if (hitInfo.transform.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
                    {
                        if (tag.m_Tag == ReflectionTagsEnum.Mirror)
                        {
                            Mirror mirror = tag.GetComponentInParent<Mirror>();
                            newMirrors.Add(mirror);

                            // Store mirror hit details
                            mirrorHitPoint = hitInfo.point;
                            mirrorHitNormal = hitInfo.normal;
                            isMirror = true;

                            // Update ray for next reflection
                            currentRayOrigin = hitInfo.point + (Vector2)(hitInfo.normal * 0.01f); // Small offset to avoid self-hit
                            currentRayDirection = Vector2.Reflect(currentRayDirection, hitInfo.normal);
                            continue; // Proceed to next reflection
                        }
                        if (tag.m_Tag == ReflectionTagsEnum.Fragment)
                        {
                            Fragment fragment = tag.GetComponentInParent<Fragment>();
                            isMirror = false;
                            newFragments.Add(fragment);
                            currentRayOrigin = hitInfo.point + (Vector2)(hitInfo.normal * 0.01f);
                            m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                           currentRayOrigin + currentRayDirection * m_MaxRayDistance);
                            // Fragments don't reflect, so stop further reflections

                            
                            continue;
                        }

                        if (tag.m_Tag == ReflectionTagsEnum.FragmentCollecter)
                        {
                                FragmentCollector fragmentCollector = tag.GetComponent<FragmentCollector>();
                            isMirror = false;

                                newFragmentCollector.Add(fragmentCollector);
                           currentRayOrigin = hitInfo.point + (Vector2)(hitInfo.normal * 0.01f);
                                m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                               currentRayOrigin + currentRayDirection * m_MaxRayDistance);
                                continue;
                        }

                    }

                    // Stop processing if not a mirror or fragment
                    break;
                }
                else
                {
                    // Handle cases where the ray doesn't hit any collider
                    if (isMirror)
                    {
                        m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                            mirrorHitPoint + Vector2.Reflect(currentRayDirection, mirrorHitNormal) * m_MaxRayDistance);
                    }
                    else
                    {
                        m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                            currentRayOrigin + currentRayDirection * m_MaxRayDistance);
                    }
                    break;
                }
            }

            // Handle Mirror Events
            foreach (var mirror in newMirrors)
            {
                if (!currentMirrors.Contains(mirror))
                {
                    OnMirrorTriggerEnter(mirror);
                }
            }

            foreach (var mirror in currentMirrors)
            {
                if (!newMirrors.Contains(mirror))
                {
                    OnMirrorTriggerExit(mirror);
                }
            }

            currentMirrors = newMirrors;

            // Handle Fragment Events
            foreach (var fragment in newFragments)
            {
                if (!currentFragments.Contains(fragment))
                {
                    OnFragmentTriggerEnter(fragment);
                }
            }

            foreach (var fragment in currentFragments)
            {
                if (!newFragments.Contains(fragment))
                {
                    OnFragmentTriggerExit(fragment);
                }
            }

            currentFragments = newFragments;

            // Handle Fragment Collector Events
            foreach (var fragmentCollector in newFragmentCollector)
            {
                if (!currentragmentCollector.Contains(fragmentCollector))
                {
                    OnFragmentCollectorEnter(fragmentCollector);
                }
            }

            foreach (var fragmentCollector in currentragmentCollector)
            {
                if (!newFragmentCollector.Contains(fragmentCollector))
                {
                    OnFragmentCollectorExit(fragmentCollector);
                }
            }

            currentragmentCollector = newFragmentCollector;
        }

        public virtual void OnMirrorTriggerEnter(Mirror mirror)
        {

            Debug.Log("Enter Mirror " + mirror.name);
            mirror.OnSunglightEnter();
        }

        public virtual void OnMirrorTriggerExit(Mirror mirror)
        {

            Debug.Log("Exit Mirror " + mirror.name);
            mirror.OnSunlightExit();


        }

        public virtual void OnFragmentTriggerEnter(Fragment fragment)
        {
            Debug.Log("Enter Fragment " + fragment.name);
            fragment.OnSunlightTriggerEnter();
        }

        public virtual void OnFragmentTriggerExit(Fragment fragment)
        {
            Debug.Log("Exit Fragment " + fragment.name);
            fragment.OnSunlightTriggerExit();
        }

        public virtual void OnFragmentCollectorEnter(FragmentCollector fragmentCollector)
        {
            Debug.Log("Enter fragmentCollector" + fragmentCollector.name);
            fragmentCollector.OnSunlightEnter();
        }

        public virtual void OnFragmentCollectorExit(FragmentCollector fragmentCollector)
        {
            Debug.Log("Exit FragmentCollecter" + fragmentCollector.name);
            fragmentCollector.OnSunlightExit();
        }
    }
}
