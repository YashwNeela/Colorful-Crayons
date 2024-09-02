using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting {
    public enum AudioLanguage
    {
        None,
        English,
        Hindi
    }

    public class AudioManager :SerializedSingleton<AudioManager>
    {

        [SerializeField] private Dictionary<AudioLanguage, AudioLocalizationSO> audioSO;
        private AudioLocalizationSO m_CurrentLocalizedAudio;
        [SerializeField] private AudioSource m_BackGroundAudioSource;
        [SerializeField] private AudioSource m_SFXAudioSource;

        private void Start()
        {
            SetAudioLanguage(AudioLanguage.Hindi);
        }

        private void SetAudioLanguage(AudioLanguage audioLanguage)
        {
            m_CurrentLocalizedAudio = audioSO[audioLanguage];
            PlayBackgroundAudio();
        }

        public void PlayIntroSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.intro.Count)];
            m_SFXAudioSource.Play();
        }

        public void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();
                

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.levelComplete.Count)];
            m_SFXAudioSource.Play();
        }


        public void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.levelFail.Count)];
            m_SFXAudioSource.Play();
        }

        public void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            m_SFXAudioSource.Play();
        }

        public void PlayLevelRetrySfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.retry.Count)];
            m_SFXAudioSource.Play();
        }

        public void PlayBackgroundAudio(bool looping = true)
        {
            m_BackGroundAudioSource.loop = looping;

            m_BackGroundAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.background.Count)];

            m_BackGroundAudioSource.Play();
        }


    }
}
