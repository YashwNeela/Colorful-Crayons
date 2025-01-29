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
    public ParticleImage particleImageLight;

    public Transform m_WaterGemContainer;


    public Image m_WaterGem;
    public ParticleImage particleImageWater;


    public Transform m_EarthGemContainer;

    public Image m_EarthGem;
    public ParticleImage particleImageEarth;


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
            particleImageLight.Play();
            break;
            case FragmentType.Water:
            m_WaterGem.color = Color.white;
            particleImageWater.Play();
            break;
            case FragmentType.Earth:
            m_EarthGem.color = Color.white;
            particleImageEarth.Play();
            break;
        }
    }

    public void OnGemUnCollected(FragmentType fragmentType)
    {
        switch(fragmentType){
            case FragmentType.Light:
            m_LightGem.color = Color.black;
            break;
            case FragmentType.Water:
            m_WaterGem.color = Color.black;
            break;
             case FragmentType.Earth:
            m_EarthGem.color = Color.black;
            break;
        }
    }
}
}
