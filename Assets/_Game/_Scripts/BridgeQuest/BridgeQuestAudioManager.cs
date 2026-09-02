using UnityEngine;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Bridge Quest's AudioManager -- the music and SFX half of the audio system.
    /// The voice-over half lives in BridgeQuestAudioMapper / BridgeQuestVoice and
    /// streams keyed mp3s off the CDN; nothing here touches that path.
    ///
    /// Modelled on RocketRunAudioManager, and for the same two reasons:
    ///
    ///  1. Background music autostarts from AudioManager.Awake via _bgMusic, and
    ///     game-state cues arrive on the GameManager static event bus.
    ///  2. Bridge Quest has no AudioLocalizationSO entry, so the base
    ///     SetAudioLanguage's audioSO[audioLanguage] lookup would throw and abort
    ///     Awake -- which silently kills the music AND stops OnEnable from ever
    ///     subscribing the SFX handlers. It is overridden below.
    ///
    /// Every base Play*Sfx that indexes AudioMapper by level is also overridden.
    /// BridgeQuestAudioMapper carries no LevelIntro/LevelComplete/LevelFail arrays
    /// (its keys are named, not indexed), so the inherited bodies would walk off the
    /// end of an empty array on the first mission.
    /// </summary>
    public class BridgeQuestAudioManager : AudioManager
    {
        [Header("Bridge Quest SFX")]
        [Tooltip("A plank landing on the span. Fires once per correct answer.")]
        [SerializeField] private AudioClip plankPlacedSfx;

        [Tooltip("The fifth plank completing the bridge, before the crossing.")]
        [SerializeField] private AudioClip bridgeCompleteSfx;

        [SerializeField] private AudioClip correctAnswerSfx;

        [Tooltip("Gentle. A wrong tap costs nothing in this game -- this must not read as a buzzer.")]
        [SerializeField] private AudioClip wrongAnswerSfx;

        [Tooltip("Played on the end screen, on the Extra source so it survives the SFX channel.")]
        [SerializeField] private AudioClip missionCompleteSfx;

        [Tooltip("Buttons: replay, next, back to Playschool.")]
        [SerializeField] private AudioClip uiTapSfx;

        [Header("Voice ducking")]
        [Tooltip("Pull the music down while a voice-over line is speaking. The BGM loop is two minutes long and otherwise sits right on top of the narration.")]
        [SerializeField] private bool duckMusicUnderVoice = true;

        [Range(0f, 1f)]
        [SerializeField] private float duckedMusicVolume = 0.25f;

        [Tooltip("Volume units per second. Low enough that the duck is heard as a dip, not a cut.")]
        [SerializeField] private float duckFadeSpeed = 4f;

        /// <summary>
        /// Set in Awake rather than resolved through AudioManager.Instance.
        /// SerializedSingleton's getter CREATES an empty GameObject when it finds no
        /// instance, and an AudioManager with null AudioSources throws the moment
        /// Awake reaches PlayBackgroundAudio. A plain cached reference simply stays
        /// null when the scene has no manager, which is what every caller here wants.
        /// </summary>
        private static BridgeQuestAudioManager active;

        /// <summary>
        /// Bridge Quest's music/SFX manager, or null when none is in the scene.
        /// Self-healing: the cache is refilled by search rather than by construction,
        /// so a scene without a manager stays silent instead of spawning a broken one.
        /// </summary>
        public static BridgeQuestAudioManager Active
        {
            get
            {
                if (active == null) active = FindObjectOfType<BridgeQuestAudioManager>();
                return active;
            }
        }

        private float fullMusicVolume = 1f;
        private AudioSource voiceSource;

        protected override void Awake()
        {
            base.Awake();

            active = this;
            if (m_BackGroundAudioSource != null) fullMusicVolume = m_BackGroundAudioSource.volume;
        }

        private void OnDestroy()
        {
            if (active == this) active = null;
        }

        /// <summary>
        /// See the class comment: the localized-clip table is unused here because every
        /// Bridge Quest cue is either a direct AudioClip below or a CDN voice key.
        /// </summary>
        protected override void SetAudioLanguage(AudioLanguage audioLanguage)
        {
            if (audioLanguage == AudioLanguage.None) return;
            m_CurretAudioLanguage = audioLanguage;
            PlayBackgroundAudio();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BridgeQuestGameManager.OnPlankPlaced += HandlePlankPlaced;
            BridgeQuestGameManager.OnBridgeComplete += HandleBridgeComplete;
            BridgeQuestGameManager.OnCorrectAnswer += HandleCorrectAnswer;
            BridgeQuestGameManager.OnWrongAnswer += HandleWrongAnswer;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            BridgeQuestGameManager.OnPlankPlaced -= HandlePlankPlaced;
            BridgeQuestGameManager.OnBridgeComplete -= HandleBridgeComplete;
            BridgeQuestGameManager.OnCorrectAnswer -= HandleCorrectAnswer;
            BridgeQuestGameManager.OnWrongAnswer -= HandleWrongAnswer;
        }

        #region Game event SFX

        private void HandlePlankPlaced(int index, int total)
        {
            PlaySfx(plankPlacedSfx);
        }

        private void HandleBridgeComplete()
        {
            PlaySfx(bridgeCompleteSfx);
        }

        private void HandleCorrectAnswer(QuestionType type)
        {
            PlaySfx(correctAnswerSfx);
        }

        private void HandleWrongAnswer(QuestionType type)
        {
            PlaySfx(wrongAnswerSfx);
        }

        /// <summary>Button feedback. Safe to call when no clip is assigned.</summary>
        public void PlayUiTap()
        {
            PlaySfx(uiTapSfx);
        }

        /// <summary>
        /// One-shot on the SFX channel. PlayOneShot rather than clip+Play so a plank
        /// landing during the previous plank's tail overlaps instead of cutting it --
        /// five correct answers in a row otherwise sounds like one clipped thud.
        /// </summary>
        private void PlaySfx(AudioClip clip)
        {
            if (clip == null || m_SFXAudioSource == null) return;
            m_SFXAudioSource.PlayOneShot(clip);
        }

        #endregion

        #region Base overrides -- see class comment

        // The mission intro voice line already covers the start of a mission.
        public override void PlayLevelStartSfx(bool overridePreviousClips = false) { }

        // No-fail game: GameLoose is never raised, and there is no fail cue to play.
        public override void PlayLevelFailSfx(bool overridePreviousClips = false) { }

        // m_CurrentLocalizedAudio is null here, so the base body would throw.
        public override void PlayLevelRetrySfx(bool overridePreviousClips = false) { }

        public override void PlayIntroSfx(bool overridePreviousClips = false) { }

        public override void PlayLevelCompleteSfx(bool overridePreviousClips = false)
        {
            if (missionCompleteSfx == null || m_ExtraAudioSource == null) return;
            if (m_ExtraAudioSource.isPlaying && !overridePreviousClips) return;
            if (overridePreviousClips && m_ExtraAudioSource.isPlaying) m_ExtraAudioSource.Stop();

            m_ExtraAudioSource.clip = missionCompleteSfx;
            m_ExtraAudioSource.Play();
        }

        public override void PlayGameCompleteSfx(bool overridePreviousClips = false)
        {
            PlayLevelCompleteSfx(overridePreviousClips);
        }

        #endregion

        #region Ducking

        private void Update()
        {
            if (!duckMusicUnderVoice || m_BackGroundAudioSource == null) return;

            // resolved lazily: RuntimeAudioLoader is a DontDestroyOnLoad singleton
            // owned by the Playschool shell and may arrive after this Awake
            if (voiceSource == null)
            {
                RuntimeAudioLoader loader = RuntimeAudioLoader.Instance;
                if (loader != null) voiceSource = loader._commonAudioSource;
            }

            bool speaking = voiceSource != null && voiceSource.isPlaying;
            float target = speaking ? duckedMusicVolume * fullMusicVolume : fullMusicVolume;

            // unscaled: the storyboard and the end screen both freeze Time.timeScale,
            // and both of them talk
            m_BackGroundAudioSource.volume = Mathf.MoveTowards(
                m_BackGroundAudioSource.volume, target, duckFadeSpeed * Time.unscaledDeltaTime);
        }

        #endregion
    }

    /// <summary>
    /// Fail-quiet entry point for UI code that wants a tap sound without holding a
    /// reference to the manager. Mirrors BridgeQuestVoice on the voice-over side.
    /// </summary>
    public static class BridgeQuestSfx
    {
        public static void Tap()
        {
            BridgeQuestAudioManager m = BridgeQuestAudioManager.Active;
            if (m != null) m.PlayUiTap();
        }
    }
}
