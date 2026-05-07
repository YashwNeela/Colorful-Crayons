using UnityEngine;
using TMKOC;

namespace TMKOC.StarLink
{
    public class StarLinkLevelManager : LevelManager
    {
        protected override void Start()
        {
            base.Start();
        }

        public override void LoadLevel(int levelIndex)
        {
            base.LoadLevel(levelIndex);
            // Additional level loading logic for StarLink can go here
        }
    }
}
