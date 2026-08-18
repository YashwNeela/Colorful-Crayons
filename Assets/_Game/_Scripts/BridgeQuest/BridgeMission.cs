using UnityEngine;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// The Level component for one mission. Six of these sit in the scene, one per
    /// mission, listed on the BridgeQuestLevelManager -- which is what makes Bridge
    /// Quest a discrete-level game and lets the base GameManager's star saving and
    /// GameWin/GameCompleted routing work untouched.
    ///
    /// The GameObject itself only needs to carry the mission's backdrop and bridge
    /// layout; everything the mission SAYS and ASKS lives in the MissionData asset,
    /// so mission seven is a new asset plus a mostly-empty GameObject.
    /// </summary>
    public class BridgeMission : Level
    {
        [Header("Mission")]
        [SerializeField] private MissionData missionData;

        [Tooltip("Left empty, found in the scene on first load.")]
        [SerializeField] private BridgeQuestFlow flow;

        public MissionData MissionData { get { return missionData; } }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();

            if (missionData == null)
            {
                Debug.LogWarning("[BridgeQuest] " + name + " has no MissionData assigned.", this);
                return;
            }

            if (flow == null) flow = FindObjectOfType<BridgeQuestFlow>();
            if (flow == null)
            {
                Debug.LogWarning("[BridgeQuest] No BridgeQuestFlow in the scene -- mission cannot start.", this);
                return;
            }

            flow.BeginMission(missionData);
        }

        public override void OnLevelUnloaded()
        {
            base.OnLevelUnloaded();
        }
    }
}
