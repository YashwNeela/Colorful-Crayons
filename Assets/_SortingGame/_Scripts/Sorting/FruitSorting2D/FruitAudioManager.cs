using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{

    public class FruitAudioManager : AudioManager
    {
        //public Action<int> PassLevelNumber;

        //int index = 0;

        //protected override void OnEnable()
        //{
        //    Gamemanager.OnFirstTimeGameStartAction += OnFirstTimeStartGame;
        //    Gamemanager.OnGameStart += OnGameStart;
        //    Gamemanager.OnGameCompleted += OnGameComplete;

        //    Gamemanager.OnGameWin += OnGameWin;
        //    Gamemanager.OnGameLoose += OnGameLoose;
        //}

        //protected override void OnDisable()
        //{
        //    Gamemanager.OnFirstTimeGameStartAction -= OnFirstTimeStartGame;
        //    Gamemanager.OnGameStart -= OnGameStart;
        //    Gamemanager.OnGameCompleted -= OnGameComplete;

        //    Gamemanager.OnGameWin -= OnGameWin;
        //    Gamemanager.OnGameLoose -= OnGameLoose;
        //}


        //private void OnGameStart()
        //{
        //    if(m_CurrentLocalizedAudio == null)
        //    {
        //        SetAudioLanguage(m_CurretAudioLanguage);
        //    }
        //    PlayLevelStartSfx(true);
        //}

        //protected override void OnGameWin()
        //{
        //    PlayLevelCompleteSfx(true);
        //    index++;
        //    Debug.Log("YES_CORRECT");
        //}
        //protected override void OnGameLoose()
        //{
        //    PlayLevelFailSfx(true);
        //    Debug.Log("NO_INCORRECT");
        //}

        //private void OnFirstTimeStartGame()
        //{
        //    Debug.Log("CHK========FirstTimeStartGame");
        //}

        //private void OnGameComplete()
        //{
        //    PlayGameCompleteSfx(true);
        //}

        //public override void PlayGameCompleteSfx(bool overridePreviousClips = false)
        //{
        //    if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
        //        return;
        //    else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
        //        m_SFXAudioSource.Stop();

        //    m_SFXAudioSource.clip = m_CurrentLocalizedAudio.gameComplete[UnityEngine.Random.Range(0, m_CurrentLocalizedAudio.gameComplete.Count)];
        //    m_SFXAudioSource.Play();
        //}

        //public override void PlayLevelStartSfx(bool overridePreviousClips = false)
        //{
        //    if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
        //        return;
        //    else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
        //        m_SFXAudioSource.Stop();

        //    if (index >= 0 && index < m_CurrentLocalizedAudio.levelStart.Count)
        //    {
        //        m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelStart[index];
        //        m_SFXAudioSource.Play();
        //    }
        //}
        //public override void PlayLevelFailSfx(bool overridePreviousClips = false)
        //{
        //    if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
        //        return;
        //    else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
        //        m_SFXAudioSource.Stop();

        //    if (index >= 0 && index < m_CurrentLocalizedAudio.levelFail.Count)
        //    {
        //        m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelFail[index];
        //        m_SFXAudioSource.Play();
        //    }
        //}
        //public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        //{
        //    if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
        //        return;
        //    else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
        //        m_SFXAudioSource.Stop();

        //    if (index >= 0 && index < m_CurrentLocalizedAudio.levelComplete.Count)
        //    {
        //        m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelComplete[index];
        //        m_SFXAudioSource.Play();
        //    }
        //}
    }

}