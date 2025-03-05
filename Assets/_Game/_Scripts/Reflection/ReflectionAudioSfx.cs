using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    [CreateAssetMenu(fileName = "RelfectionAudio", menuName = "ScriptableObject/ReflectionAudio/Sfx")]
    public class ReflectionAudioSfx : ScriptableObject
    {
        public List<AudioClip> m_LightReflects;
    }
}