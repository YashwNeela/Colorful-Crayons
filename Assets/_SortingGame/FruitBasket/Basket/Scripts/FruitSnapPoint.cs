using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
    public class FruitSnapPoint : SnapPoint
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        public void SetBasketType(BasketType type)
        {
            m_BasketType = type;
        }
    }
}
