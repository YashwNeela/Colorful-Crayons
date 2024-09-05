using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting.ToySorting
{
    [Flags]
    public enum ToyType
    {
        None = 0,
        Ball = 1 << 0,
        Bats = 1 << 1,
        Teddy = 1 << 2,
        BaseBall = 1 << 3,

        BeachBall = 1<<4, FootBall = 1<<5, SoocerBall = 1<<6, TenisBall = 1<<7, VollyBall = 1<<8, BaseBallbat = 1<<9, Batmintan = 1<<10, CricketBat = 1<<11, HockeyStick = 1<<12,
        TableTennisBat = 1<<13


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
            if (toyType.HasFlag(ToyType.None))
            {
                selectedColors.Add(ColorCodes.red);
            }
            if (toyType.HasFlag(ToyType.None))
            {
                selectedColors.Add(ColorCodes.yellow);
            }
            if (toyType.HasFlag(ToyType.None) || toyType.HasFlag(ToyType.None))
            {
                selectedColors.Add(ColorCodes.green);
            }
            if (toyType.HasFlag(ToyType.None))
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
                ToySnapPoint toySnapPoint = snapPoint as ToySnapPoint;
                ToyType f = (collectible as Toy).ToyType;
                //  basketType & fruitType
                // bool y = fruitSnapPoint.BasketType & (collectible as Fruit).BasketType;

                if (CanToyBePutInToyBox(toySnapPoint.ToyType, f) &&
                    !toySnapPoint.IsOccupied)
                {
                    collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform;
                    collectible.transform.localPosition = Vector3.zero;
                    collectible.transform.localRotation = Quaternion.identity;
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();

                    break;
                }
              
            }
        }

        private bool CanToyBePutInToyBox(ToyType toyBoxType, ToyType toyType)
        {
            // Check if the basket type has all the required flags of the fruit type
            return (toyBoxType & toyType) != 0;
        }
}
}
