using UnityEngine;

namespace TMKOC
{
    /// <summary>
    /// RocketRun's AudioManager. Follows the same base-class event pattern as
    /// CarSortingAudioManager (background music autostarts via AudioManager.Awake,
    /// game-state SFX driven by the GameManager static event bus) but, since RocketRun
    /// has no discrete levels, every Play*Sfx override plays a direct clip instead of
    /// going through AudioMapper/RuntimeAudioLoader (which are keyed by level index).
    /// </summary>
    public class RocketRunAudioManager : AudioManager
    {
        [Header("RocketRun SFX")]
        [SerializeField] private AudioClip correctPickupSfx;
        [SerializeField] private AudioClip incorrectPickupSfx;
        [SerializeField] private AudioClip crashSfx;
        [SerializeField] private AudioClip missionCompleteSfx;

        protected override void OnEnable()
        {
            base.OnEnable();
            RocketRunGameManager.OnCorrectPickup += HandleCorrectPickup;
            RocketRunGameManager.OnIncorrectPickup += HandleIncorrectPickup;
            RocketRunGameManager.OnPlayerCrashed += HandlePlayerCrashed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            RocketRunGameManager.OnCorrectPickup -= HandleCorrectPickup;
            RocketRunGameManager.OnIncorrectPickup -= HandleIncorrectPickup;
            RocketRunGameManager.OnPlayerCrashed -= HandlePlayerCrashed;
        }

        private void HandleCorrectPickup()
        {
            if (correctPickupSfx != null) PlayAudio(correctPickupSfx, SFXAudioSource, true, true);
        }

        private void HandleIncorrectPickup()
        {
            if (incorrectPickupSfx != null) PlayAudio(incorrectPickupSfx, SFXAudioSource, true, true);
        }

        private void HandlePlayerCrashed()
        {
            if (crashSfx != null) PlayAudio(crashSfx, SFXAudioSource, true, true);
        }

        // No per-level intro concept for a continuous run -- background music already covers it.
        public override void PlayLevelStartSfx(bool overridePreviousClips = false) { }

        // Crashes are cosmetic, never routed through GameLoose -- nothing to play here.
        public override void PlayLevelFailSfx(bool overridePreviousClips = false) { }

        public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            if (missionCompleteSfx == null) return;
            if (ExtraAudioSource.isPlaying && !overridePreviousClips) return;
            if (overridePreviousClips && ExtraAudioSource.isPlaying) ExtraAudioSource.Stop();
            ExtraAudioSource.clip = missionCompleteSfx;
            ExtraAudioSource.Play();
        }

        public override void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            PlayLevelCompleteSfx(overridePreviousClips);
        }
    }
}
