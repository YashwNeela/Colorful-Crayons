using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.FruitSorting;
using UnityEngine;
namespace TMKOC.Sorting
{
    public class FruitSortingAudioManager : AudioManager
    {
        public void PlayFruitNameAudio(BasketType basketType, bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = (m_CurrentLocalizedAudio as FruitSortingAudio)
            .FruitNameDict[basketType];
            m_SFXAudioSource.Play();
        }
    }
}
