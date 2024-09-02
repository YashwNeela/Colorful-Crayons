using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting
{
    [CreateAssetMenu(fileName = "LocalizedAudio", menuName = "ScriptableObject/LocalizedAudio")]

    public class AudioLocalizationSO : ScriptableObject
    {
        public AudioLanguage audioLanguage;

        public List<AudioClip> intro, background, levelComplete, levelFail, gameComplete, retry;

       
    }
}
