using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace TMKOC.Reflection{
public class ShootLaser : MonoBehaviour
{
    public Material m_Material;
    LaserBeam m_Beam;

    // Update is called once per frame
    void Update()
    {
        Destroy(GameObject.Find("Laser Beam"));
        m_Beam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, m_Material);
    }
}
}
