using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//Sun liaadasd
namespace TMKOC.Sorting
{
    public enum AudioLanguage
    {
        None,
        English,
        Hindi
    }

    public class AudioManager : SerializedSingleton<AudioManager>
    {
        [SerializeField] protected AudioLanguage m_CurretAudioLanguage;
        [SerializeField] protected Dictionary<AudioLanguage, AudioLocalizationSO> audioSO;
        protected AudioLocalizationSO m_CurrentLocalizedAudio;
        [SerializeField] protected AudioSource m_BackGroundAudioSource;
        [SerializeField] protected AudioSource m_SFXAudioSource;

        [SerializeField] protected AudioSource m_ExtraAudioSource;

        protected virtual void OnEnable()
        {
            Gamemanager.OnFirstTimeGameStartAction += OnFirstTimeGameStart;
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameWin += OnGameWin;
            Gamemanager.OnGameLoose += OnGameLoose;
            Gamemanager.OnRightAnswerAction += OnRightAnswer;
            Gamemanager.OnWrongAnswerAction += OnWrongAnswer;
        }

        private void OnGameStart()
        {
            PlayLevelStartSfx();
        }

        private void OnFirstTimeGameStart()
        {
            PlayIntroSfx();
        }

        protected virtual void OnWrongAnswer()
        {
            PlayWrongAnswerAudio();
        }

        protected virtual void OnRightAnswer()
        {
            PlayRightAnswerAudio();
        }

        protected virtual void OnGameLoose()
        {
            PlayLevelFailSfx();
        }

        protected virtual void OnDisable()
        {
            Gamemanager.OnFirstTimeGameStartAction -= OnFirstTimeGameStart;
            Gamemanager.OnGameStart -= OnGameStart;

            Gamemanager.OnGameWin -= OnGameWin;
            Gamemanager.OnGameLoose -= OnGameLoose;
            Gamemanager.OnRightAnswerAction -= OnRightAnswer;
            Gamemanager.OnWrongAnswerAction -= OnWrongAnswer;

        }



        protected virtual void OnGameWin()
        {
            PlayLevelCompleteSfx();
        }

        protected virtual void Start()
        {
            #if PLAYSCHOOL_MAIN
            switch (PlayerPrefs.SetString("PlaySchoolLanguage" , currntLanguage);)
            {
                case "English":
                SetAudioLanguage(AudioLanguage.English);
                break;
                case "Hindi":
                SetAudioLanguage(AudioLanguage.Hindi);
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
            m_CurrentLocalizedAudio = audioSO[audioLanguage];
            PlayBackgroundAudio();
        }


        public virtual void PlayIntroSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.intro[Random.Range(0, m_CurrentLocalizedAudio.intro.Count)];
            m_SFXAudioSource.Play();
        }

        public virtual void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying)
                return;

            if (Random.Range(0f, 1f) < 0.5f)
            {
                m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelStart[Random.Range(0, m_CurrentLocalizedAudio.levelStart.Count)];
                m_SFXAudioSource.Play();
            }
        }

        public virtual void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();


            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelComplete[Random.Range(0, m_CurrentLocalizedAudio.levelComplete.Count)];
            m_SFXAudioSource.Play();
        }


        public virtual void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[Random.Range(0, m_CurrentLocalizedAudio.levelFail.Count)];
            m_SFXAudioSource.Play();
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

        public virtual void PlayLevelRetrySfx(bool overridePreviousClips = false)
        {
            if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
                m_SFXAudioSource.Stop();

            m_SFXAudioSource.clip = m_CurrentLocalizedAudio.retry[Random.Range(0, m_CurrentLocalizedAudio.retry.Count)];
            m_SFXAudioSource.Play();
        }

        public virtual void PlayBackgroundAudio(bool looping = true)
        {
            m_BackGroundAudioSource.loop = looping;

            m_BackGroundAudioSource.clip = m_CurrentLocalizedAudio.background[Random.Range(0, m_CurrentLocalizedAudio.background.Count)];

            m_BackGroundAudioSource.Play();
        }

        public virtual void PlayRightAnswerAudio(bool overridePreviousClips = false)
        {
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_ExtraAudioSource.isPlaying)
                m_ExtraAudioSource.Stop();

            m_ExtraAudioSource.clip = m_CurrentLocalizedAudio.rightAnswer[Random.Range(0, m_CurrentLocalizedAudio.rightAnswer.Count)];
            m_ExtraAudioSource.Play();
        }

        public virtual void PlayWrongAnswerAudio(bool overridePreviousClips = false)
        {
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_ExtraAudioSource.isPlaying)
                m_ExtraAudioSource.Stop();

            m_ExtraAudioSource.clip = m_CurrentLocalizedAudio.wrongAnswer[Random.Range(0, m_CurrentLocalizedAudio.wrongAnswer.Count)];
            m_ExtraAudioSource.Play();
        }




    }
}
