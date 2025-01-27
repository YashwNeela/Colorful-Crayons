using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.PlantLifeCycle;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Reflection{
public class GemsUI : SerializedSingleton<GemsUI>
{
    public Transform m_LigthGemContainer;

    public Image m_LightGem;

    public Image m_HeartGem;

    public Image m_LeafeGem;

    public void OnEnable()
    {
        GameManager.OnGameStart += OnGameStart;

    }

    private void OnGameStart()
    {
            m_LightGem.color = Color.black;
            m_HeartGem.color = Color.black;
            m_LeafeGem.color = Color.black;
    }

     public void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStart;

    }

    public void OnGemCollected(FragmentType fragmentType)
    {
        switch(fragmentType){
            case FragmentType.Diamond:
            m_LightGem.color = Color.white;
            break;
        }
    }
}
}
