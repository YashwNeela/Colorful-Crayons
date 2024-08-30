using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
    
[CreateAssetMenu(fileName = "FruitBasketLable", menuName = "ScriptableObject/FruitBasketLable")]
public class FruitBasketLableSO : SerializedScriptableObject
{
    
    [SerializeField]
    private Dictionary<BasketType, Sprite> m_BasketLableTextures;

    [HideInInspector]
    public Dictionary<BasketType,Sprite> BasketLableTextures => m_BasketLableTextures;
    
}

}