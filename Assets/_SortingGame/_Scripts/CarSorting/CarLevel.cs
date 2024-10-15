using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC.Sorting.CarSorting
{
    [System.Serializable]
    public struct CollectibleSelectionData
    {
        public CollectibleDataSO collectibleDataSO;
        public int noOfCollectibleToSpawn;
    }

    public class CarLevel : Level
    {
        
        [SerializeField] private CollectibleSelectionData[] collectibleSelectionData;
        [SerializeField]
        public UnityEvent onGameWinCarLevel;

        [SerializeField] private float m_LayoutSpacing;

        protected override void OnGameStart()
        {
            base.OnGameStart();
            CollectibleSelectionUI.Instance.ClearSelectionUI();

            
            for(int i = 0;i<collectibleSelectionData.Length;i++)
            {
                
               CollectibleSelectionUI.Instance.AddData(collectibleSelectionData[i].collectibleDataSO,collectibleSelectionData[i].noOfCollectibleToSpawn,m_LayoutSpacing);
            }

            


        }

        protected override void OnLevelCompleteCheck()
        {
             StartCoroutine(Co_OnLevelCompletedCheck());
        }

        private IEnumerator Co_OnLevelCompletedCheck()
        {
            onLevelCompleteCheck?.Invoke();
            if(m_CurrentScore == m_ScoreRequiredToCompleteTheLevel){
                Gamemanager.Instance.GameOver();
                onGameWinCarLevel?.Invoke();
                yield return new WaitForSeconds(2);
               Gamemanager.Instance.GameWin();
            }else
            {
                Gamemanager.Instance.GameOver();
                Gamemanager.Instance.GameLoose();
            }
        }

        protected override void OnGameRestart()
        {
            base.OnGameRestart();
            CollectibleSelectionUI.Instance.ClearSelectionUI();
            
        }

    }
}
