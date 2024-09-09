using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        protected override void HandleDragStart()
        {
            base.HandleDragStart();
                FruitSortingAudioManager Instance = FruitSortingAudioManager.Instance as FruitSortingAudioManager;

            if(m_BasketType.HasFlag(BasketType.Chery))
            {
                Instance.PlayFruitNameAudio(BasketType.Chery);
            }else if(m_BasketType.HasFlag(BasketType.Tomato))
            {
                Instance.PlayFruitNameAudio(BasketType.Tomato);

            }else if(m_BasketType.HasFlag(BasketType.BeetRoot))
            {
                Instance.PlayFruitNameAudio(BasketType.BeetRoot);

            }else if(m_BasketType.HasFlag(BasketType.Apple))
            {
                Instance.PlayFruitNameAudio(BasketType.Apple);

            }else if(m_BasketType.HasFlag(BasketType.Banana))
            {
                Instance.PlayFruitNameAudio(BasketType.Banana);

            }else if(m_BasketType.HasFlag(BasketType.StarFruit))
            {
                Instance.PlayFruitNameAudio(BasketType.StarFruit);

            }else if(m_BasketType.HasFlag(BasketType.Mango))
            {
                Instance.PlayFruitNameAudio(BasketType.Mango);

            }else if(m_BasketType.HasFlag(BasketType.WaterMelon))
            {
                Instance.PlayFruitNameAudio(BasketType.WaterMelon);

            }else if(m_BasketType.HasFlag(BasketType.Pear))
            {
                Instance.PlayFruitNameAudio(BasketType.Pear);

            }else if(m_BasketType.HasFlag(BasketType.Papaya))
            {
                Instance.PlayFruitNameAudio(BasketType.Papaya);

            }else if(m_BasketType.HasFlag(BasketType.Apricot))
            {
                Instance.PlayFruitNameAudio(BasketType.Apricot);

            }else if(m_BasketType.HasFlag(BasketType.GrapeFruit))
            {
                Instance.PlayFruitNameAudio(BasketType.GrapeFruit);

            }else if(m_BasketType.HasFlag(BasketType.Guava))
            {
                Instance.PlayFruitNameAudio(BasketType.Guava);

            }else if(m_BasketType.HasFlag(BasketType.OrangeFruit))
            {
                Instance.PlayFruitNameAudio(BasketType.OrangeFruit);

            }else if(m_BasketType.HasFlag(BasketType.Avacado))
            {
                Instance.PlayFruitNameAudio(BasketType.Avacado);

            }else if(m_BasketType.HasFlag(BasketType.GreenApple))
            {
                Instance.PlayFruitNameAudio(BasketType.GreenApple);

            }

           

        }


        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            FruitBasket collectorBox = other.GetComponent<Collector>() as FruitBasket;

            if (collectorBox != null)
            {
                // Check if the collector box can accept this fruit based on the flags
                if (CanFruitBePutInBasket(collectorBox.BasketType, m_BasketType))
                {
                    m_ValidCollector = collectorBox; // Valid collector found
                    m_IsTryingToPlaceWrong = false;  // Reset the flag for placing wrong
                }
                else
                {
                    m_IsTryingToPlaceWrong = true;  // Set the flag for wrong placement
                    m_ValidCollector = null;        // Reset valid collector as it's incorrect
                }
            }
        }

        // Method to check if a fruit can be put in a basket based on flags
        private bool CanFruitBePutInBasket(BasketType basketType, BasketType fruitType)
        {
            // Check if the basket type has all the required flags of the fruit type
             return (basketType & fruitType) != 0;
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
                // Use CanFruitBePutInBasket method to check if the basket can accept the fruit
                if (!CanFruitBePutInBasket(collectorBox.BasketType, m_BasketType))
                {
                    m_IsTryingToPlaceWrong = true;  // Set the flag for wrong placement
                }
                
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