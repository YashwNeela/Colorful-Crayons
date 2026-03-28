using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

//Sun liaadasd tap tap siddhart tap tap
namespace TMKOC.Sorting
{
    public class SortingAudioManager : AudioManager
    {
        public AudioClip _correctMusic;
        public AudioClip _incorrectMusic;


        protected override void OnEnable()
        {
            base.OnEnable();
            SortingGameManager.OnRightAnswerAction += OnRightAnswer;
            SortingGameManager.OnWrongAnswerAction += OnWrongAnswer;
        }


        protected virtual void OnWrongAnswer()
        {
            PlayWrongAnswerAudio();
        }

        protected virtual void OnRightAnswer()
        {
            PlayRightAnswerAudio();
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            SortingGameManager.OnRightAnswerAction -= OnRightAnswer;
            SortingGameManager.OnWrongAnswerAction -= OnWrongAnswer;
        }



        public virtual void PlayRightAnswerAudio(bool overridePreviousClips = false)
        {
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_ExtraAudioSource.isPlaying)
                m_ExtraAudioSource.Stop();

            //m_ExtraAudioSource.clip = m_CurrentLocalizedAudio.rightAnswer[Random.Range(0, m_CurrentLocalizedAudio.rightAnswer.Count)];

            if (_correctMusic != null)
            {
                m_ExtraAudioSource.clip = _correctMusic;
            }

            m_ExtraAudioSource.Play();
        }

        public virtual void PlayWrongAnswerAudio(bool overridePreviousClips = false)
        {
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips)
                return;
            else if (overridePreviousClips && m_ExtraAudioSource.isPlaying)
                m_ExtraAudioSource.Stop();

            //m_ExtraAudioSource.clip = m_CurrentLocalizedAudio.wrongAnswer[Random.Range(0, m_CurrentLocalizedAudio.wrongAnswer.Count)];

            if (_incorrectMusic != null)
            {
                m_ExtraAudioSource.clip = _incorrectMusic;
            }
            m_ExtraAudioSource.Play();
        }

        public override void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.LevelIntro[LevelManager.Instance.CurrentLevelIndex]);
            //Debug.Log($"len: {len}");
            //m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelIntro[LevelManager.Instance.CurrentLevelIndex];
            //m_SFXAudioSource.Play();
        }

        public override void PlayLevelFailSfx(bool overridePreviousClips = false)
        {
            //m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[LevelManager.Instance.CurrentLevelIndex];
            //m_SFXAudioSource.Play();

            RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.LevelFail[LevelManager.Instance.CurrentLevelIndex]);
        }

        public override void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            //m_SFXAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
            //m_SFXAudioSource.Play();

            RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.GameComplete);

        }


    }
}
