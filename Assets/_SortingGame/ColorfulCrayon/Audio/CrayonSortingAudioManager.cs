using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class CrayonSortingAudioManager : AudioManager
    {
        public void PlayColorNameAudio(CrayonColor color, bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = (m_CurrentLocalizedAudio as CrayonSortingAudio)
            .CrayonColorNameDict[color];
            m_SFXAudioSource.Play();
        
        }

    }

}