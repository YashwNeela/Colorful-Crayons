using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class CrayonSortingAudioManager : SortingAudioManager
    {
        public void PlayColorNameAudio(CrayonColor color, bool overridePreviousClips = false)
        {
            // if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
            //     return;
            // else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
            //     m_SFXAudioSource.Stop();

            // m_SFXAudioSource.clip = (m_CurrentLocalizedAudio as CrayonSortingAudio)
            // .CrayonColorNameDict[color];
            // m_SFXAudioSource.Play();

        }

        public override void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelIntro[LevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
        }

        public override void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[LevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
        }

        public override void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            m_SFXAudioSource.Play();
        }

        public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelComplete[LevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
        }

    }

}