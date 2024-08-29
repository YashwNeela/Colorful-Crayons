using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
    [Flags]
    public enum BasketType
    {
        None = 0,
        Red = 1 << 0,
        Yellow = 1 << 1,
        Green = 1 << 2,
        Blue = 1 << 3
    }
    public class FruitBasket : Collector
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        private DOTweenAnimation m_OnBasketEnteredAnimation;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            m_OnBasketEnteredAnimation = GetComponent<DOTweenAnimation>();

            // Set the colors of the box based on the selected CrayonColor flags
            SetBoxColorsBasedOnEnum(m_BasketType);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Gamemanager.OnRightAnswerAction += OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction += OnWrongAnswer;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gamemanager.OnRightAnswerAction -= OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction -= OnWrongAnswer;
        }

        private void OnRightAnswerAction()
        {
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        private void OnWrongAnswer()
        {
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

         private void SetBoxColorsBasedOnEnum(BasketType basketType)
        {
            List<Color> selectedColors = new List<Color>();

            // Check each flag and add corresponding colors from ColorCodes
            if (basketType.HasFlag(BasketType.Red))
            {
                selectedColors.Add(ColorCodes.red);
            }
            if (basketType.HasFlag(BasketType.Yellow))
            {
                selectedColors.Add(ColorCodes.yellow);
            }
            if (basketType.HasFlag(BasketType.Green))
            {
                selectedColors.Add(ColorCodes.green);
            }
            if (basketType.HasFlag(BasketType.Blue))
            {
                selectedColors.Add(ColorCodes.blue);
            }

            m_Renderer.materials[1].color = selectedColors[0];
        }

        protected override void OnItemCollected(SnapPoint snapPoint)
        {
            base.OnItemCollected(snapPoint);
            m_StartCollector.SetEmitter(snapPoint.transform);
            m_StartCollector.PlayParticle();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            Debug.Log("Collectible entered");
            m_OnBasketEnteredAnimation.DOPlayForward();
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            base.OnCollectibleExited(collectible);
            Debug.Log("Collectible Exited");
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }
        
    }
}
