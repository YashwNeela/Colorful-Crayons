using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.CarSorting
{
    public class CarSnapPoint : SnapPoint
    {
        [SerializeField] private CarType m_CarType;

        public CarType CarType => m_CarType;

        public override bool HasValidCollectible()
        {
            if(m_CurrentCollectible != null &&(m_CurrentCollectible as CarTire).CarType.HasFlag(m_CarType))
                return true;
            else
                return false;
        }

    }

}