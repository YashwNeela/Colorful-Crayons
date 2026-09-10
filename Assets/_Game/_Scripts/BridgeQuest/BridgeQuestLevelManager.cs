using UnityEngine;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Bridge Quest's level list -- the six missions.
    ///
    /// LoadLevel is overridden rather than inherited for two reasons, both bugs in
    /// the base implementation that only bite a game with real level switching:
    ///
    ///  1. Base LoadLevel calls OnLevelUnloaded on levels[currentLevelIndex] inside
    ///     the deactivation loop, so the OUTGOING level is unloaded once per level in
    ///     the list rather than once, and the other levels are never unloaded at all.
    ///  2. Base LoadLevel dereferences m_LevelText and m_TipText unconditionally, so
    ///     a scene without a level counter throws on the first load. Bridge Quest
    ///     deliberately has no on-screen counter -- the bridge is the progress
    ///     display -- so those fields are legitimately empty here.
    /// </summary>
    public class BridgeQuestLevelManager : LevelManager
    {
        public override void LoadLevel(int levelIndex)
        {
            if (levels == null || levelIndex < 0 || levelIndex >= levels.Count) return;

            // unload exactly the level we are leaving, once
            if (currentLevelIndex >= 0
                && currentLevelIndex < levels.Count
                && levels[currentLevelIndex] != null)
            {
                Level outgoing = levels[currentLevelIndex].GetComponent<Level>();
                if (outgoing != null) outgoing.OnLevelUnloaded();
            }

            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] != null) levels[i].SetActive(false);
            }

            currentLevelIndex = levelIndex;

            GameObject go = levels[currentLevelIndex];
            if (go == null) return;

            go.SetActive(true);

            Level incoming = go.GetComponent<Level>();
            if (incoming != null)
            {
                incoming.OnLevelLoaded();
                if (m_TipText != null) m_TipText.text = incoming.m_Tip;
            }

            // optional in Bridge Quest -- see the class comment
            if (m_LevelText != null)
            {
                m_LevelText.text = (currentLevelIndex + 1) + "/" + MaxLevels;
            }
        }
    }
}
