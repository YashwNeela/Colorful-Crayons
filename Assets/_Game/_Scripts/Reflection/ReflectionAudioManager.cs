using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{
public class ReflectionAudioManager : AudioManager
{
    public override void PlayIntroSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.intro[Random.Range(0, m_CurrentLocalizedAudio.intro.Count)];
            m_SFXAudioSource.Play();
        }

        public override void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            
        }
}
}
