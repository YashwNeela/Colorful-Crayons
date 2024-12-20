using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

[CreateAssetMenu(fileName = "CrayonSortingAudio", menuName = "ScriptableObject/CrayonSortingAudio")]
public class CrayonSortingAudio : SortingAudioLocalizationSO
{
    [SerializeField] protected Dictionary<CrayonColor, AudioClip> crayonColorNameDict;

    public Dictionary<CrayonColor,AudioClip> CrayonColorNameDict => crayonColorNameDict;
    
}
