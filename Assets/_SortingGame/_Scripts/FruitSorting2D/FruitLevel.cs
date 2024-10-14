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

        protected override void SetScoreRequiredToCompleteTheLevel()
        {
            base.m_CurrentScore = 0;

            base.m_ScoreRequiredToCompleteTheLevel = this.m_ScoreRequiredToCompleteTheLevel;
        }
    }
}
