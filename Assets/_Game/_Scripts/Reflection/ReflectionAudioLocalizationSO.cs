using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMKOC.Sorting;


namespace TMKOC.Reflection{
[CreateAssetMenu(fileName = "RelfectionAudio", menuName = "ScriptableObject/ReflectionAudio")]

public class ReflectionAudioLocalizationSO : AudioLocalizationSO
{
    public List<AudioClip> tutorialAudio;

    public List<AudioClip> gemCollection;
}
}
