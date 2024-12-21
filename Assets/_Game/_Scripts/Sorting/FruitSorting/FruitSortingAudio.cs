using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.FruitSorting;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitSortingAudio", menuName = "ScriptableObject/FruitSortingAudio")]
public class FruitSortingAudio : SortingAudioLocalizationSO
{
     [SerializeField] protected Dictionary<BasketType, AudioClip> fruitNameDict;

    public Dictionary<BasketType,AudioClip> FruitNameDict => fruitNameDict;
}
