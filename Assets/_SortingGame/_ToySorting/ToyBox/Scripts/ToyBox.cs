using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting
{
    [Flags]
    public enum ToyType
    {
        None = 0,
        Red = 1 << 0,
        Yellow = 1 << 1,
        Green = 1 << 2,
        Orange = 1 << 3,

        Chery = 1<<4, Tomato = 1<<5, BeetRoot = 1<<6, Apple = 1<<7, Banana = 1<<8, StarFruit = 1<<9, Mango = 1<<10, WaterMelon = 1<<11, Pear = 1<<12,
        Papaya = 1<<13, Apricot = 1<<14, GrapeFruit = 1<<15, Guava = 1<<16, OrangeFruit = 1<<17, Avacado = 1<<18, GreenApple = 1<<19


    }
public class ToyBox : Collector
{
    [SerializeField] private ToyType m_ToyType;

    public ToyType ToyType => m_ToyType;

    private Renderer m_Renderer;

    private StarCollectorParticleImage m_StartCollector;

    private DOTweenAnimation m_OnToyBoxEnteredAnimation;

    
    protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            m_OnToyBoxEnteredAnimation = GetComponent<DOTweenAnimation>();

            // Set the colors of the box based on the selected CrayonColor flags
            SetBoxColorsBasedOnEnum(m_ToyType);
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
            m_OnToyBoxEnteredAnimation.DOComplete();
            m_OnToyBoxEnteredAnimation.DOPlayBackwards();
        }

        private void OnWrongAnswer()
        {
            m_OnToyBoxEnteredAnimation.DOComplete();
            m_OnToyBoxEnteredAnimation.DOPlayBackwards();
        }

        
         private void SetBoxColorsBasedOnEnum(ToyType toyType)
        {
            List<Color> selectedColors = new List<Color>();

            // Check each flag and add corresponding colors from ColorCodes
            if (toyType.HasFlag(ToyType.Red))
            {
                selectedColors.Add(ColorCodes.red);
            }
            if (toyType.HasFlag(ToyType.Yellow))
            {
                selectedColors.Add(ColorCodes.yellow);
            }
            if (toyType.HasFlag(ToyType.Green) || toyType.HasFlag(BasketType.WaterMelon))
            {
                selectedColors.Add(ColorCodes.green);
            }
            if (toyType.HasFlag(ToyType.Orange))
            {
                selectedColors.Add(ColorCodes.orange);
            }

            

            m_Renderer.materials[1].color = selectedColors[0];
        }
    
        protected override void OnItemCollected(SnapPoint snapPoint)
        {
            base.OnItemCollected(snapPoint);
            m_StartCollector.SetEmitter(snapPoint.transform);
            m_StartCollector.PlayParticle();
            m_OnToyBoxEnteredAnimation.DOPlayBackwards();
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            m_OnToyBoxEnteredAnimation.DOPlayBackwards();
        }

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            Debug.Log("Collectible entered");
            m_OnToyBoxEnteredAnimation.DOPlayForward();
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            base.OnCollectibleExited(collectible);
            Debug.Log("Collectible Exited");
            m_OnToyBoxEnteredAnimation.DOComplete();
            m_OnToyBoxEnteredAnimation.DOPlayBackwards();
        }

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                  //  collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                }
            }
        }
}
}