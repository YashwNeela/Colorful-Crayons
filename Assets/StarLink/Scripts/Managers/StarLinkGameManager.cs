using UnityEngine;
using TMKOC;

namespace TMKOC.StarLink
{
    public class StarLinkGameManager : GameManager
    {
        public override void Start()
        {
            base.Start();
        }

        public override void FirstTimeGameStart()
        {
            base.FirstTimeGameStart();
            GameStart(StarLinkGameManager.Instance.levelNumber);

        }

        public override void GameStart(int level)
        {
            base.GameStart(level);
            // Custom start logic for StarLink, if any.
        }

        public override void GameWin()
        {
            base.GameWin();
            // Custom win logic for StarLink
        }

        public override void GameLoose()
        {
            base.GameLoose();
            // Custom lose logic for StarLink
        }
    }
}
