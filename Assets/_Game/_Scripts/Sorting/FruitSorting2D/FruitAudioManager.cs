using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{

    public class FruitAudioManager : SortingAudioManager
    {
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
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[UnityEngine.Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            m_SFXAudioSource.Play();
        }

    }

}