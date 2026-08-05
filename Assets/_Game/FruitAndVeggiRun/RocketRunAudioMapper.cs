using UnityEngine;

namespace TMKOC
{
    /// <summary>
    /// Every voice-over key RocketRun asks RuntimeAudioLoader for. The strings here
    /// are the "voiceover_title" column of the Fruit &amp; Veggie Run VO sheet, which is
    /// also the mp3 filename inside
    /// d2r38fn3ydtrfq.cloudfront.net/fruit_veggie_run_game/{Language}.zip.
    ///
    /// Defaults are baked in so a fresh component works without hand-filling the
    /// inspector, but every field stays serialized so the keys can be retuned
    /// without a code change.
    /// </summary>
    public class RocketRunAudioMapper : AudioMapper
    {
        [Header("Audio bundle")]
        [Tooltip("CDN folder the clips live in. Matches the activity_name column of the VO sheet.")]
        public string CategoryName = "fruit_veggie_run_game";

        [Header("Tutorial")]
        public string TutorialFly = "tut_fly";
        public string TutorialCollect = "tut_collect";
        public string TutorialWrongFruit = "tut_wrong_fruit";
        public string TutorialWater = "tut_water";

        [Header("Pickups (one is picked at random)")]
        public string[] CorrectPickup = { "pickup_correct_1", "pickup_correct_2", "pickup_correct_3" };
        public string[] WrongPickup = { "pickup_wrong_1", "pickup_wrong_2", "pickup_wrong_3" };
        public string[] Crash = { "crash_water_1", "crash_water_2" };

        [Header("Shopping list")]
        [Tooltip("Prefixed onto the produce name, e.g. apple -> find_apple.")]
        public string FindPrefix = "find_";
        [Tooltip("Prefixed onto the produce name, e.g. apple -> done_apple.")]
        public string DonePrefix = "done_";

        [Header("Screens")]
        public string RetryScreen = "retry_screen";
        public string WinScreen = "win_screen";

        /// <summary>"Now find the apples!" for the item the player is currently hunting.</summary>
        public string GetFindAudio(string itemName)
        {
            return string.IsNullOrEmpty(itemName) ? null : FindPrefix + itemName;
        }

        /// <summary>"All the apples are in the basket!" for a finished shopping-list item.</summary>
        public string GetDoneAudio(string itemName)
        {
            return string.IsNullOrEmpty(itemName) ? null : DonePrefix + itemName;
        }

        public string GetRandomCorrectPickup() { return Pick(CorrectPickup); }
        public string GetRandomWrongPickup() { return Pick(WrongPickup); }
        public string GetRandomCrash() { return Pick(Crash); }

        private static string Pick(string[] pool)
        {
            if (pool == null || pool.Length == 0) return null;
            return pool[Random.Range(0, pool.Length)];
        }
    }

    /// <summary>
    /// Thin, fail-quiet wrapper around RuntimeAudioLoader.
    ///
    /// The loader is a DontDestroyOnLoad singleton created by the Playschool shell,
    /// so it simply is not there when the RocketRun scene is played on its own, and
    /// its PlayRuntimeAudio dereferences the clip for its length off-editor -- which
    /// throws when a key is missing from the bundle. Both cases should cost silence,
    /// never an exception in the middle of a game.
    /// </summary>
    public static class RocketRunVoice
    {
        public static void Play(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            RuntimeAudioLoader loader = RuntimeAudioLoader.Instance;
            if (loader == null || loader._commonAudioSource == null) return;
            if (loader.GetClip(key) == null) return;

            loader.PlayRuntimeAudio(key);
        }

        /// <summary>Convenience for the RocketRun mapper, which is what every call site wants.</summary>
        public static RocketRunAudioMapper Mapper
        {
            get { return AudioMapper.Instance as RocketRunAudioMapper; }
        }
    }
}
