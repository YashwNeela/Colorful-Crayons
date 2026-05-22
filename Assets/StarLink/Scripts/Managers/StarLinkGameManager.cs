using UnityEngine;
using TMKOC;

namespace TMKOC.StarLink
{
    public class StarLinkGameManager : GameManager
    {

        public TutorialData tutorialData;
        public override void Start()
        {
            base.Start();
            #if PLAYSCHOOL_MAIN
            PlayschoolCommon.Instance.SpawnplayschoolWinLosePanel();
            #endif
        }

        public override void FirstTimeGameStart()
        {
            base.FirstTimeGameStart();
            GameStart(StarLinkGameManager.Instance.levelNumber);

        }

        public override void GameStart(int level)
        {
            base.GameStart(level);
            if(level== 0){
        
            //Start tutorial

            TMKOC.StarLink.TutorialManager.Instance.StartTutorial(tutorialData);
            }

            // Custom start logic for StarLink, if any.
        }

        public override void GameWin()
        {
            base.GameWin();
            // WinLosePanelScript.Instance.ShowNextLevelPopUp(()=>
            // {
            //     m_CurrentGameState = GameState.Win;
            // OnGameWin?.Invoke();

            // if (LevelManager.Instance.CurrentLevelIndex + 1 >= LevelManager.Instance.MaxLevels)
            // {
            //     GameCompleted();
            //     return;
            // }
            // GameNotCompleted();
            // });
            
        }

        public override void GameLoose()
        {
            base.GameLoose();
            // Custom lose logic for StarLink
        }

        public override void GameCompleted()
        {
            base.GameCompleted();
            #if PLAYSCHOOL_MAIN
                    EffectParticleControll.Instance.SpawnGameEndPanel();
                  //  GameManager.Instance.SoundManager.PlayFinalOutro();
                    GameOverEndPanel.Instance.AddTheListnerRetryGame();
                    return;
#endif

        }
    }
}
