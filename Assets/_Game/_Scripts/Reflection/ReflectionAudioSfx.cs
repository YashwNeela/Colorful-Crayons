using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    [CreateAssetMenu(fileName = "RelfectionAudio", menuName = "ScriptableObject/ReflectionAudio/Sfx")]
    public class ReflectionAudioSfx : ScriptableObject
    {
        public List<AudioClip> m_LightReflects;

        public List<AudioClip> m_Jump;

        public List<AudioClip> m_Walk;

        public AudioClip m_ButtonClick;

        public List<AudioClip> m_OnMirrorInteracted;

        public AudioClip m_FragmentCollected;
    }
}