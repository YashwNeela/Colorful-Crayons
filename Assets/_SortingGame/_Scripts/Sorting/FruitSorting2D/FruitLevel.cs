using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitLevel : Level
    {
        [SerializeField] private new int m_ScoreRequiredToCompleteTheLevel;
        [SerializeField] private FruitType m_FruitType;

        protected override void OnGameStart()
        {
            base.OnGameStart();
            SetScoreRequiredToCompleteTheLevel();
        }

        protected override void OnLevelCompleteCheck()
        {
            //base.OnLevelCompleteCheck();

            int totalCount = 0;

            foreach (Collector collector in Collectors)
            {
                for (int i = 0; i < collector.SnapPoints.Length; i++)
                {
                    if (collector.SnapPoints[i].IsOccupied)
                    {
                        totalCount++;
                    }
                }
            }

            onLevelCompleteCheck?.Invoke();

            if (m_CurrentScore == m_ScoreRequiredToCompleteTheLevel && totalCount == m_ScoreRequiredToCompleteTheLevel)
            {
                SortingGameManager.Instance.GameOver();
                SortingGameManager.Instance.GameWin();
            }
            else
            {
                SortingGameManager.Instance.GameOver();
                SortingGameManager.Instance.GameLoose();
            }
        }

        protected override void SetScoreRequiredToCompleteTheLevel()
        {
            base.m_CurrentScore = 0;

            base.m_ScoreRequiredToCompleteTheLevel = this.m_ScoreRequiredToCompleteTheLevel;
        }
    }
}
