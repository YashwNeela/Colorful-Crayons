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
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_ExtraAudioSource.isPlaying)
                m_ExtraAudioSource.Stop();

            m_ExtraAudioSource.clip = (m_CurrentLocalizedAudio as CrayonSortingAudio)
            .CrayonColorNameDict[color];
            m_ExtraAudioSource.Play();
        }

    }

}