using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TMKOC{
public class ParallaxController : MonoBehaviour
{
    Transform m_Cam;
    Vector3 m_CamStartPos;
    float m_Distance;

    GameObject[] m_Backgrounds;
    Material[] m_Mats;
    float[] m_BackSpeed;

    float m_FarthestBack;

    [Range(0.01f,0.05f)]
    public float m_ParallaxSpeed;

    void Start()
    {
        m_Cam = Camera.main.transform;
        m_CamStartPos = m_Cam.position;

        int backCount = transform.childCount;
        m_Mats = new Material[backCount];
        m_BackSpeed = new float[backCount];
        m_Backgrounds = new GameObject[backCount];

        for(int i = 0;i<backCount;i++)
        {
            m_Backgrounds[i] = transform.GetChild(i).gameObject;
            m_Mats[i] = m_Backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        for(int i = 0;i< backCount; i++)
        {
            if((m_Backgrounds[i].transform.position.z-m_Cam.position.z)   > m_FarthestBack)
            {
                m_FarthestBack = m_Backgrounds[i].transform.position.z - m_Cam.position.z;
            }
        }

        for(int i = 0;i<backCount;i++)
        {
            m_BackSpeed[i] = 1-(m_Backgrounds[i].transform.position.z - m_Cam.position.z)/m_FarthestBack;
        }
    }

    private void LateUpdate() {
       //Invoke(nameof(UpdateParallax),0.1f);
       UpdateParallax();
    }
    
    private  void UpdateParallax() 
    {
        m_Distance = m_Cam.position.x - m_CamStartPos.x;
        transform.position = Vector3.Lerp(transform.position, new Vector3(m_Cam.position.x,transform.position.y,0), 0.1f);
        for(int i =0;i< m_Backgrounds.Length;i++)
        {
            float speed = m_BackSpeed[i] * m_ParallaxSpeed;
            m_Mats[i].SetTextureOffset("_MainTex", new Vector2(m_Distance,0) * speed);
        }
    }

}
}