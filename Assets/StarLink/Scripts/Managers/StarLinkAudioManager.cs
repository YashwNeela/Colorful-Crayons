using UnityEngine;

namespace TMKOC.StarLink
{
public class StarLinkAudioManager : AudioManager
{
     public override void PlayLevelStartSfx(bool overridePreviousClips = false)
        {
            // if (m_SFXAudioSource.isPlaying)
            //     return;

            // if (Random.Range(0f, 1f) < 0.5f)
            // {

            //     //m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelIntro[LevelManager.Instance.CurrentLevelIndex];
            //     //m_SFXAudioSource.Play();

            //     RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.LevelIntro[LevelManager.Instance.CurrentLevelIndex]);
            // }
        }


        public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            
            // if (m_SFXAudioSource.isPlaying && !overridePreviousClips)
            //     return;
            // else if (overridePreviousClips && m_SFXAudioSource.isPlaying)
            //     m_SFXAudioSource.Stop();


            // m_SFXAudioSource.clip = m_CurrentLocalizedAudio.levelComplete[LevelManager.Instance.CurrentLevelIndex];
            // m_SFXAudioSource.Play();

            // RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.LevelComplete[LevelManager.Instance.CurrentLevelIndex]);

        }

        
}
}