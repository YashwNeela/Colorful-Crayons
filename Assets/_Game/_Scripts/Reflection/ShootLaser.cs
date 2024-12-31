using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace TMKOC.Reflection{
public class ShootLaser : MonoBehaviour
{
    public LineRenderer m_LaserLine;

    public int m_Reflections;
    public float m_MaxRayDistance;
    public LayerMask m_LayerDetection;
    public float m_RotationSpeed;
    public Material m_Material;
    LaserBeam m_Beam;

    private void Start()
    {
        Physics2D.queriesStartInColliders = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_LaserLine.positionCount = 1;
        m_LaserLine.SetPosition(0,transform.position);

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position,transform.right,m_MaxRayDistance,m_LayerDetection);

        Ray2D ray = new Ray2D(transform.position,transform.right);
        bool isMirror = false;  
        Vector2 mirrorHitPoint = Vector2.zero;
        Vector2 mirrorHitNormal = Vector2.zero;

        for(int i = 0;i< m_Reflections ;i++)
        {
            m_LaserLine.positionCount += 1;
            if(hitInfo.collider != null)
            {
                m_LaserLine.SetPosition(m_LaserLine.positionCount - 1, hitInfo.point - ray.direction * -0.1f);
                isMirror =false;
                if(hitInfo.transform.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
                {
                    if(tag.m_Tag == ReflectionTagsEnum.Mirror){
                    mirrorHitPoint = (Vector2)hitInfo.point;
                    mirrorHitNormal = (Vector2)hitInfo.normal;
                    hitInfo = Physics2D.Raycast((Vector2)hitInfo.point  - ray.direction * -0.1f, Vector2.Reflect(hitInfo.point  - ray.direction * -0.1f,
                    hitInfo.normal),m_MaxRayDistance,m_LayerDetection);
                    isMirror = true;
                    }
                }else

                    break;
            }else
            {
                if(isMirror)
                {
                    m_LaserLine.SetPosition(m_LaserLine.positionCount - 1,mirrorHitPoint + Vector2.Reflect(mirrorHitPoint,mirrorHitNormal) * m_MaxRayDistance);
                    break;
                }else
                {
                    m_LaserLine.SetPosition(m_LaserLine.positionCount - 1,transform.position + transform.right * m_MaxRayDistance);
                    break;
                }
            }
        }
    
    
    
    
    
    
    
    }
}
}
