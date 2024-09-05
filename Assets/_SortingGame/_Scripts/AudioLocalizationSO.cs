using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting
{
    [CreateAssetMenu(fileName = "LocalizedAudio", menuName = "ScriptableObject/LocalizedAudio")]

    public class AudioLocalizationSO : SerializedScriptableObject
    {
        public AudioLanguage audioLanguage;

        public List<AudioClip> intro, background,levelStart, levelComplete, levelFail, gameComplete, retry, rightAnswer, wrongAnswer;

       
    }
}
