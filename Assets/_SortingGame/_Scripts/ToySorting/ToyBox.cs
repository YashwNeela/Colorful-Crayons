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

        BeachBall = 1 << 4, FootBall = 1 << 5, SoocerBall = 1 << 6, TenisBall = 1 << 7, VollyBall = 1 << 8, BaseBallbat = 1 << 9, Batmintan = 1 << 10, CricketBat = 1 << 11, HockeyStick = 1 << 12,
        TableTennisBat = 1 << 13, Teddy1Blue = 1 << 14, Teddy1Green = 1 << 15, Teddy1Pink = 1 << 16, Teddy1Red = 1 << 17, Teddy1Yellow = 1 << 18, Teddy2Blue = 1 << 19, Teddy2Green = 1 << 20, Teddy2Pink = 1 << 21, Teddy2Red = 1 << 22, Teddy2Yellow = 1 << 23,
        Teddy3Blue = 1 << 24, Teddy3Green = 1 << 25, Teddy3Pink = 1 << 26, Teddy3Red = 1 << 27, Teddy3Yellow = 1 << 28


    }
    public class ToyBox : Collector
    {
        [SerializeField] private ToyType m_ToyType;

        public ToyType ToyType => m_ToyType;

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        [SerializeField] private ToyBoxLableSO m_ToyBoxLableSO;

        private DOTweenAnimation m_OnToyBoxEnteredAnimation;


        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            m_OnToyBoxEnteredAnimation = GetComponent<DOTweenAnimation>();

            // Set the colors of the box based on the selected CrayonColor flags
            SetToyBoxTextures(m_ToyType);
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


        private void SetToyBoxTextures(ToyType toyType)
        {

            if (ToyType.HasFlag(ToyType.BeachBall))
            {
                // Handle BeachBall
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.BeachBall].texture;

            }
            else if (ToyType.HasFlag(ToyType.FootBall))
            {
                // Handle FootBall
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.FootBall].texture;

            }
            else if (ToyType.HasFlag(ToyType.SoocerBall))
            {
                // Handle SoocerBall
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.SoocerBall].texture;

            }
            else if (ToyType.HasFlag(ToyType.TenisBall))
            {
                // Handle TenisBall
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.TenisBall].texture;

            }
            else if (ToyType.HasFlag(ToyType.VollyBall))
            {
                // Handle VollyBall
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.VollyBall].texture;

            }
            else if (ToyType.HasFlag(ToyType.BaseBallbat))
            {
                // Handle BaseBallbat
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.BaseBallbat].texture;

            }
            else if (ToyType.HasFlag(ToyType.Batmintan))
            {
                // Handle Batmintan
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Batmintan].texture;

            }
            else if (ToyType.HasFlag(ToyType.CricketBat))
            {
                // Handle CricketBat
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.CricketBat].texture;

            }
            else if (ToyType.HasFlag(ToyType.HockeyStick))
            {
                // Handle HockeyStick
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.HockeyStick].texture;

            }
            else if (ToyType.HasFlag(ToyType.TableTennisBat))
            {
                // Handle TableTennisBat
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.TableTennisBat].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy1Blue))
            {
                // Handle Teddy1Blue
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy1Blue].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy1Green))
            {
                // Handle Teddy1Green
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy1Green].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy1Pink))
            {
                // Handle Teddy1Pink
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy1Pink].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy1Red))
            {
                // Handle Teddy1Red
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy1Red].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy1Yellow))
            {
                // Handle Teddy1Yellow
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy1Yellow].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy2Blue))
            {
                // Handle Teddy2Blue
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy2Blue].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy2Green))
            {
                // Handle Teddy2Green
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy2Green].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy2Pink))
            {
                // Handle Teddy2Pink
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy2Pink].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy2Red))
            {
                // Handle Teddy2Red
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy2Red].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy2Yellow))
            {
                // Handle Teddy2Yellow
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy2Yellow].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy3Blue))
            {
                // Handle Teddy3Blue
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy3Blue].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy3Green))
            {
                // Handle Teddy3Green
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy3Green].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy3Pink))
            {
                // Handle Teddy3Pink
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy3Pink].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy3Red))
            {
                // Handle Teddy3Red
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy3Red].texture;

            }
            else if (ToyType.HasFlag(ToyType.Teddy3Yellow))
            {
                // Handle Teddy3Yellow
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[ToyType.Teddy3Yellow].texture;

            } else
            {
            m_Renderer.materials[1].mainTexture = m_ToyBoxLableSO.ToyBoxLableTextures[toyType].texture;
                
            }
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
                    collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(false);
                    collectible.transform.parent = snapPoint.transform;
                    collectible.transform.localPosition = Vector3.zero;
                    //   collectible.transform.localRotation = Quaternion.identity;
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
