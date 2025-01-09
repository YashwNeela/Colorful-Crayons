using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMKOC{
public class Parallax : MonoBehaviour
{
    Material m_Mat;
    float m_Distance;

    [Range(0,0.5f)]
    public float m_Speed = 0.2f;

    void Start()
    {
        m_Mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        m_Distance += Time.deltaTime * m_Speed;
        m_Mat.SetTextureOffset("_MainTex", Vector2.right * m_Distance);
    }

}
}
