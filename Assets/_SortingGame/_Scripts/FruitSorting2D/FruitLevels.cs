using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;


namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitLevels : Level
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

        private void OnFruitSelected(FruitType fruit)
        {
            if (m_FruitType.HasFlag(fruit))
                base.m_CurrentScore++;
            else
                base.m_CurrentScore += 0.3f;
        }

        private void OnFruitDeselected(FruitType fruit)
        {
            if (m_FruitType.HasFlag(fruit))
                base.m_CurrentScore--;
            else
                base.m_CurrentScore -= 0.3f;
        }
    }
}
