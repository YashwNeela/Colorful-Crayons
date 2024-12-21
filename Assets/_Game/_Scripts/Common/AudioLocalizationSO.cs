using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TMKOC
{
    [CreateAssetMenu(fileName = "LocalizedAudio", menuName = "ScriptableObject/LocalizedAudio")]

    public class AudioLocalizationSO : SerializedScriptableObject
    {
         public AudioLanguage audioLanguage;

        public List<AudioClip> intro, background,levelStart, levelComplete, levelFail,levelIntro, gameComplete, retry, rightAnswer, wrongAnswer;
    }
}