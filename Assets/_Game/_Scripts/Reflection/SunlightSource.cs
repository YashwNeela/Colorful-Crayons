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

        public float m_StartZRotation;



        private void Start()
        {
            Physics2D.queriesStartInColliders = false;
            Invoke(nameof(SetStartRotation),1);
        }

        private void SetStartRotation()
        {
            transform.Rotate(0,0,m_StartZRotation);
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

                // Perform raycast
                RaycastHit2D hitInfo = Physics2D.Raycast(currentRayOrigin, currentRayDirection, m_MaxRayDistance, m_LayerDetection);

                
                if (hitInfo.collider != null)
                {
                    // Update the ray's position
                    m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1, hitInfo.point);

                    // Check the object type
                    if (hitInfo.transform.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
                    {
                        if (tag.m_Tag == ReflectionTagsEnum.Mirror)
                        {
                            Mirror mirror = tag.GetComponentInParent<Mirror>();
                            newMirrors.Add(mirror);

                            // Reflect the ray
                            currentRayOrigin = hitInfo.point + (Vector2)(hitInfo.normal); // Offset to avoid self-hit
                            currentRayDirection = Vector2.Reflect(currentRayDirection, hitInfo.normal);
                            continue; // Continue to the next reflection
                        }
                        else if (tag.m_Tag == ReflectionTagsEnum.Fragment)
                        {
                            Fragment fragment = tag.GetComponentInParent<Fragment>();
                            newFragments.Add(fragment);

                            // Update ray origin to continue through the fragment
                            currentRayOrigin = hitInfo.point;
                             currentRayOrigin = hitInfo.point + (Vector2)(currentRayDirection); // Small offset forward

                            continue; // Continue propagating
                        }
                        else if (tag.m_Tag == ReflectionTagsEnum.FragmentCollecter)
                        {
                            FragmentCollector fragmentCollector = tag.GetComponentInParent<FragmentCollector>();
                            newFragmentCollector.Add(fragmentCollector);

                            // Update ray origin to continue through the fragment
                          //  currentRayOrigin = hitInfo.point /*+ (Vector2)(currentRayDirection * 0.01f)*/;
                            continue; // Continue propagating
                        }


                    }

                    // If the object is not a mirror or fragment, stop further reflections
                    break;
                }
                else
                {
                    // If no collider is hit, extend the ray to its maximum distance
                    m_SunlightLine.SetPosition(m_SunlightLine.positionCount - 1,
                        currentRayOrigin + currentRayDirection * m_MaxRayDistance);
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
                    Debug.Log("Fragment Enter");
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
