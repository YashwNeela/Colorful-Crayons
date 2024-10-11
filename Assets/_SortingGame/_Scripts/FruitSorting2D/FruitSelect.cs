using DG.Tweening;
using System;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitSelect : Collectible
    {
        public Action<FruitType> OnFruitSelected;
        public Action<FruitType> OnFruitDeselected;

        [SerializeField] private bool m_IsSelected;
        [SerializeField] private FruitType m_FruitType;

        private void FruitSelected()
        {
            transform.DOScale(1.2f, 0.25f);
            OnFruitSelected?.Invoke(m_FruitType);
        }

        private void FruitDeselected()
        {
            transform.DOScale(1f, 0.25f);
            OnFruitDeselected?.Invoke(m_FruitType);
        }
    }
}
