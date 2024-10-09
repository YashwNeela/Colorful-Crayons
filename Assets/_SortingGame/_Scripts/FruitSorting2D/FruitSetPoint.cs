using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitSetPoint : SnapPoint
    {
        [SerializeField] private FruitType m_FruitType;

        public FruitType FruitType => m_FruitType;

        public override bool HasValidCollectible()
        {
            if (m_CurrentCollectible != null && (m_CurrentCollectible as Fruit2D).FruitType.HasFlag(m_FruitType))
                return true;
            else
                return false;
        }
    }
}