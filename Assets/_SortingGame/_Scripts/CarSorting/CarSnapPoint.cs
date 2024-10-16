using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting.CarSorting
{
    public class CarSnapPoint : SnapPoint
    {
        [SerializeField] private CarType m_CarType;

        public CarType CarType => m_CarType;

        public bool m_CanSnap = true;

        public bool m_ShouldEnableOtherSnapPointsAfterCollectible;

        [ShowIf(nameof(m_ShouldEnableOtherSnapPointsAfterCollectible))]
        public CarSnapPoint[] snapPoints;

        


        public override bool HasValidCollectible()
        {
            if(m_CurrentCollectible != null &&(m_CurrentCollectible as CarTire).CarType.HasFlag(m_CarType))
                return true;
            else
                return false;
        }

        public override void SetCollectible(Collectible collectible)
        {
            base.SetCollectible(collectible);
            if(m_ShouldEnableOtherSnapPointsAfterCollectible)
            {
                for(int i = 0;i<snapPoints.Length;i++)
                {
                    snapPoints[i].m_CanSnap = true;
                }
                GetComponent<Draggable>().m_CanDrag = false;
            GetComponent<Collider2D>().enabled = false;
            }

            
        }

    }

}