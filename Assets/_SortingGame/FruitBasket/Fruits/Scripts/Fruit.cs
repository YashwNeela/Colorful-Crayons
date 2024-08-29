using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
    public class Fruit : Collectible
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        private Renderer m_Renderer;


        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();



        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            FruitBasket collectorBox = other.GetComponent<Collector>() as FruitBasket;
            if (collectorBox != null)
            {
                if (BasketType.HasFlag(collectorBox.BasketType))
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
            FruitBasket collectorBox = other.GetComponent<Collector>() as FruitBasket;
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
            FruitBasket collectorBox = other.GetComponent<Collector>() as FruitBasket;
            if (collectorBox != null)
            {
                if (!BasketType.HasFlag(collectorBox.BasketType))
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