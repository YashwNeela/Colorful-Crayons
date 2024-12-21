using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

[CreateAssetMenu(fileName = "CarSortingAudio", menuName = "ScriptableObject/CarSortingAudio")]
public class CarSortingAudio : SortingAudioLocalizationSO
{
    public AudioClip wheelsAttached;
    public AudioClip muscleCarAudio;
    public AudioClip normalCarAudio;

    public AudioClip sportsCarAudio;
}
