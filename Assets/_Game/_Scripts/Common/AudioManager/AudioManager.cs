using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC
{
    public enum AudioLanguage
    {
        None,
        English,
        Hindi,
        Tamil
    }
    public class AudioManager : SerializedSingleton<AudioManager>
    {
        #region  Audio Sources
        [SerializeField] protected Dictionary<AudioLanguage, AudioLocalizationSO> audioSO;

        [SerializeField] protected AudioLanguage m_CurretAudioLanguage;

        protected AudioLocalizationSO m_CurrentLocalizedAudio;

        public AudioLocalizationSO CurrentLocalizedAudio => m_CurrentLocalizedAudio;

        [SerializeField] protected AudioSource m_BackGroundAudioSource;

        public AudioSource BackgroundAudioSource => m_BackGroundAudioSource;

        [SerializeField] protected AudioSource m_SFXAudioSource;

        public AudioSource SFXAudioSource => m_SFXAudioSource;

        [SerializeField] protected AudioSource m_ExtraAudioSource;

        public AudioSource ExtraAudioSource => m_ExtraAudioSource;

        #endregion

        protected override void Awake()
        {
            base.Awake();
#if PLAYSCHOOL_MAIN
            switch (PlayerPrefs.GetString("PlayschoolLanguageAudio"))
            {
                case "EnglishUS":
                SetAudioLanguage(AudioLanguage.English);
                break;
                case "Hindi":
                SetAudioLanguage(AudioLanguage.Hindi);
                break;
                case "Tamil":
                SetAudioLanguage(AudioLanguage.Tamil);
                break;
                default:
                SetAudioLanguage(AudioLanguage.English);
                break;
            }
#else
            SetAudioLanguage(m_CurretAudioLanguage);
#endif

            PlayBackgroundAudio();
        }
        protected virtual void SetAudioLanguage(AudioLanguage audioLanguage)
        {
            if(audioLanguage == AudioLanguage.None)
                return;
            m_CurrentLocalizedAudio = audioSO[audioLanguage];
            PlayBackgroundAudio();
        }

        public virtual void PlayBackgroundAudio(bool looping = true)
        {
            m_BackGroundAudioSource.loop = looping;

            m_BackGroundAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.background.Count)];

            m_BackGroundAudioSource.Play();
        }
        protected virtual void OnEnable()
        {
            GameManager.OnFirstTimeGameStartAction += OnFirstTimeGameStart;
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameWin += OnGameWin;
            GameManager.OnGameLoose += OnGameLoose;
            GameManager.OnGameCompleted += OnGameCompleted;

        }

        protected virtual void OnDisable()
        {
            GameManager.OnFirstTimeGameStartAction -= OnFirstTimeGameStart;
            GameManager.OnGameStart -= OnGameStart;
            GameManager.OnGameCompleted -= OnGameCompleted;

            GameManager.OnGameWin -= OnGameWin;
            GameManager.OnGameLoose -= OnGameLoose;

        }
        protected virtual void OnFirstTimeGameStart()
        {
            PlayIntroSfx();
        }

        public virtual void PlayIntroSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            //m_SFXAudioSource.clip = m_CurrentLocalizedAudio.intro[Random.Range(0, m_CurrentLocalizedAudio.intro.Count)];
          //  m_SFXAudioSource.Play();
        }

        protected virtual void OnGameStart()
        {
            PlayLevelStartSfx();
        }

        public virtual void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            //if (Random.Range(0f, 1f) < 0.5f)
            //{
            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelIntro[SortingLevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
            //}
        }

        protected virtual void OnGameLoose()
        {
            PlayLevelFailSfx();
        }

        public virtual void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[SortingLevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
        }

        protected virtual void OnGameCompleted()
        {
            // m_ExtraAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            //m_ExtraAudioSource.Play();
        }

        protected virtual void OnGameWin()
        {
            PlayLevelCompleteSfx();
        }

        public virtual void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            m_SFXAudioSource.Play();
        }

        public virtual void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();


            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelComplete[SortingLevelManager.Instance.CurrentLevelIndex];
            m_SFXAudioSource.Play();
        }

        public virtual void PlayLevelRetrySfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.retry[Random.Range(0, m_CurrentLocalizedAudio.retry.Count)];
            m_SFXAudioSource.Play();
        }

        public virtual void PlayAudio(AudioClip clip, AudioSource audioSource,
       bool overridePreviousClips = false, bool isPlayOneShot = false)
        {
            if (audioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = clip;
            if (!isPlayOneShot)
                audioSource.Play();
            else
                audioSource.PlayOneShot(clip);
        }


    }
}
