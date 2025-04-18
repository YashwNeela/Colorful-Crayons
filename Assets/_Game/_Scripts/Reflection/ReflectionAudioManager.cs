using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class ReflectionAudioManager : AudioManager
    {
        [SerializeField] private ReflectionAudioSfx m_ReflectionAudioSfx;

        public ReflectionAudioSfx ReflectionAudioSfx => m_ReflectionAudioSfx;
        protected override void OnEnable()
        {
            base.OnEnable();
            GameManager.OnSplashScreenAction += OnSplashScreen;
        }

        private void OnSplashScreen()
        {
            PlayIntroSfx();
        }

        protected override void OnFirstTimeGameStart()
        {
           // SetAudioLanguage(AudioLanguage.English);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameManager.OnSplashScreenAction -= OnSplashScreen;

        }

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

        public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {

        }

        public override void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[Random.Range(0, m_CurrentLocalizedAudio.levelFail.Count)];
            m_SFXAudioSource.Play();
        }
    }
}
