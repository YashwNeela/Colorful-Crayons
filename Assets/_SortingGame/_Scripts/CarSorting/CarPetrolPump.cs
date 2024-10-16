using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC.Sorting.CarSorting
{
    public class CarPetrolPump : CarTire
    {
        protected override void HandleCollectorOnTriggerEnter(Component collider)
        {
            BaseHandleCollectorOnTriggerEnter(collider);
             Car collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as Car;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as Car;

            if (collectorBox != null)
            {
                // if (collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                // {
                //     m_ValidCollector = collectorBox;
                // }
                // else
                //     m_IsTryingToPlaceWrong = true;
                if(collectorBox.GetValidSnapPoint(this) != null)
                    m_ValidCollector = collectorBox;
            }
        }

        protected override void HandleDragEnd()
        {
             if (m_IsPlacedInsideCollector)
            {
                draggable.ResetToStartDraggingValues();
                return;
            }
            if (m_CurrentCollector != null && m_CurrentCollector.IsSlotAvailable())
            {
                if (m_ValidCollector != null)
                {

                    if ((m_CurrentCollector as Car).CarType.HasFlag(m_CarType))
                        m_CurrentCollector.SnapCollectibleToCollector(this,m_ValidCollector.GetValidSnapPoint(this), () => OnPlacedCorrectly());

                }


            }else
            {
                m_CanDestroy = true;
                CanDestoryRef = StartCoroutine(Co_Destroy());
                
            }
        }
    }
}
