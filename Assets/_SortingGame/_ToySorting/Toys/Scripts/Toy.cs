using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
public class Toy : Collectible
{
   [SerializeField] private ToyType m_ToyType;

        public ToyType ToyType => m_ToyType;

        private Renderer m_Renderer;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            ToyBox collectorBox = other.GetComponent<Collector>() as ToyBox;
            if (collectorBox != null)
            {
                if (ToyType.HasFlag(collectorBox.ToyType))
                {
                    m_ValidCollector = collectorBox;
                }
                else
                    m_IsTryingToPlaceWrong = true;
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            ToyBox collectorBox = other.GetComponent<Collector>() as ToyBox;
            if (collectorBox != null)
            {
                if (collectorBox == m_ValidCollector)
                    m_ValidCollector = null;

                m_IsTryingToPlaceWrong = false;

            }
        }

        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
            ToyBox collectorBox = other.GetComponent<Collector>() as ToyBox;
            if (collectorBox != null)
            {
                if (!ToyType.HasFlag(collectorBox.ToyType))
                    m_IsTryingToPlaceWrong = true;

            }
        }

        protected override void OnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
            Gamemanager.Instance.RightAnswer();
        }

        protected override void PlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);
            Gamemanager.Instance.WrongAnswer();
        }

}
}