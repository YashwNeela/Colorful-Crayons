using UnityEngine;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// One band of the difficulty ramp.
    ///
    /// Stages are not separate levels and are not measured in distance -- the run is
    /// one continuous flight, and the player moves into the next band by finishing
    /// items off the shopping list. A band therefore says two things: how much of the
    /// list it covers, and what the world should look like while it is in force.
    /// </summary>
    [System.Serializable]
    public class StageConfig
    {
        [Tooltip("Inspector label only. Has no effect on play.")]
        public string label = "Stage";

        [Header("Mission")]
        [Tooltip("How many shopping-list items this band covers. The band ends once they are all in the basket.")]
        public int itemCount = 1;

        [Tooltip("How many of those items are hunted at the same time. 1 = one fruit on the HUD, 2 = two side by side.")]
        public int activeTargets = 1;

        [Tooltip("Whether fruits that are NOT on the list mix in as decoys. Off means every pickup on screen is one the player wants.")]
        public bool decoys;

        [Header("World")]
        [Range(0f, 1f)]
        [Tooltip("Odds that any one segment is water. 0 keeps the whole band dry.")]
        public float waterChance;

        [Range(0f, 1f)]
        [Tooltip("Odds that a segment which is due a floating platform actually gets one. Spacing is handled separately on LevelBuilder, which never lets two share the screen -- this only thins them out further.")]
        public float platformChance = 1f;

        [Tooltip("How many pickups may share the screen. 1 means the next fruit only appears as the last leaves -- the calmest, clearest setting for the opening bands.")]
        public int maxPickupsOnScreen = 1;


        [Header("Birds")]
        [Tooltip("Whether birds fly through this band as moving obstacles.")]
        public bool birds;

        [Range(0f, 1f)]
        [Tooltip("Odds a segment spawns a bird, once birds are switched on.")]
        public float birdChance;

        [Tooltip("Seconds of flight at the START of this band with birds but no fruit at all, so the player learns to dodge before being asked to collect again. 0 skips the interlude.")]
        public float birdIntroSeconds;

        [Tooltip("Seconds of ordinary play after this band opens before birds turn up at all. Gives the player time to settle into whatever else the band introduced first. 0 = birds from the word go.")]
        public float birdsAfterSeconds;
    }
}
