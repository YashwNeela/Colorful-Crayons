using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMKOC.PlantLifeCycle;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Reflection{
public class GemsUI : SerializedSingleton<GemsUI>
{
    public Transform m_LigthGemContainer;

    public Image m_LightGem;

    public Sprite m_LightStar;
    public ParticleImage particleImageLight;
    
    [Space(10)]
    public Transform m_WaterGemContainer;


    public Image m_WaterGem;
    public Sprite m_WaterStar;

    public ParticleImage particleImageWater;

    [Space(10)]

    public Transform m_EarthGemContainer;

    public Image m_EarthGem;
    public Sprite m_EarthStar;

    public ParticleImage particleImageEarth;

    [Space(10)]

    public Transform m_MixedGemContainer;

    public Image m_MixedGem;
    public Sprite m_MixedStar;

    public ParticleImage particleImageMixed;


    public void OnEnable()
    {
        GameManager.OnFirstTimeGameStartAction += OnFirstTimeGameStartAction;

    }

    private void OnFirstTimeGameStartAction()
    {
            m_LightGem.color = Color.black;
            m_WaterGem.color = Color.black;
            m_EarthGem.color = Color.black;
    }

     public void OnDisable()
    {
                GameManager.OnFirstTimeGameStartAction -= OnFirstTimeGameStartAction;


    }

    public void OnGemCollected(FragmentType fragmentType)
    {
        switch(fragmentType){
            case FragmentType.Light:
            m_LightGem.color = Color.white;
            m_LightGem.fillAmount = 1f;
            particleImageLight.Play();
            break;
            case FragmentType.Water:
            m_WaterGem.color = Color.white;
            m_WaterGem.fillAmount = 1f;

            particleImageWater.Play();
            break;
            case FragmentType.Earth:
            m_EarthGem.color = Color.white;
            m_EarthGem.fillAmount = 1f;

            particleImageEarth.Play();
            break;

            case FragmentType.Mixed:
            m_MixedGem.color = Color.white;
            m_MixedGem.fillAmount = 1f;

            particleImageMixed.Play();
            break;
        }
    }

    public void OnGemUnCollected(FragmentType fragmentType)
    {
        switch(fragmentType){
            case FragmentType.Light:
            m_LightGem.color = Color.black;
            m_LightGem.fillAmount = 0f;

            break;
            case FragmentType.Water:
            m_WaterGem.color = Color.black;
            m_WaterGem.fillAmount = 0f;

            break;
             case FragmentType.Earth:
            m_EarthGem.color = Color.black;
            m_EarthGem.fillAmount = 0f;
            
            break;

            case FragmentType.Mixed:
            m_MixedGem.color = Color.black;
            m_MixedGem.fillAmount = 0f;
            
            break;
        }
    }
}
}
