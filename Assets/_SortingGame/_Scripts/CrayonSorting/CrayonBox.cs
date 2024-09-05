using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMKOC.Sorting;

namespace TMKOC.Sorting.ColorfulCrayons
{
    [Flags]
    public enum CrayonColor
    {
        None = 0,
        Red = 1 << 0,
        Yellow = 1 << 1,
        Green = 1 << 2,
        Blue = 1 << 3
    }

    public class CrayonBox : Collector
    {
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;

        private Renderer m_Renderer;
        private StarCollectorParticleImage m_StartCollector;
        private DOTweenAnimation m_OnCrayonEnteredAnimation;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            m_OnCrayonEnteredAnimation = GetComponent<DOTweenAnimation>();

            // Set the colors of the box based on the selected CrayonColor flags
            SetBoxColorsBasedOnEnum(m_CrayonColor);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Gamemanager.OnRightAnswerAction += OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction += OnWrongAnswer;
        }

        private void OnRightAnswerAction()
        {
            m_OnCrayonEnteredAnimation.DOComplete();
            m_OnCrayonEnteredAnimation.DOPlayBackwards();
        }

        private void OnWrongAnswer()
        {
            m_OnCrayonEnteredAnimation.DOComplete();
            m_OnCrayonEnteredAnimation.DOPlayBackwards();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gamemanager.OnRightAnswerAction -= OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction -= OnWrongAnswer;
        }

        // Sets the colors of the materials based on the CrayonColor enum flags
        private void SetBoxColorsBasedOnEnum(CrayonColor crayonColor)
        {
            List<Color> selectedColors = new List<Color>();

            // Check each flag and add corresponding colors from ColorCodes
            if (crayonColor.HasFlag(CrayonColor.Red))
            {
                selectedColors.Add(ColorCodes.red);
            }
            if (crayonColor.HasFlag(CrayonColor.Yellow))
            {
                selectedColors.Add(ColorCodes.yellow);
            }
            if (crayonColor.HasFlag(CrayonColor.Green))
            {
                selectedColors.Add(ColorCodes.green);
            }
            if (crayonColor.HasFlag(CrayonColor.Blue))
            {
                selectedColors.Add(ColorCodes.blue);
            }

            // Set the colors of the materials based on the selected colors
            if (selectedColors.Count > 0)
            {
                m_Renderer.materials[0].color = selectedColors[0]; // First color
            }
            if (selectedColors.Count > 1)
            {
                m_Renderer.materials[1].color = selectedColors[1]; // Second color
            }else
            {
                m_Renderer.materials[1].color = selectedColors[0]; // Second color

            }
        }

        protected override void OnItemCollected(SnapPoint snapPoint)
        {
            base.OnItemCollected(snapPoint);
            m_StartCollector.SetEmitter(snapPoint.transform);
            m_StartCollector.PlayParticle();
            m_OnCrayonEnteredAnimation.DOPlayBackwards();
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            m_OnCrayonEnteredAnimation.DOPlayBackwards();
        }

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            Debug.Log("Collectible entered");
            m_OnCrayonEnteredAnimation.DOPlayForward();
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            base.OnCollectibleExited(collectible);
            Debug.Log("Collectible Exited");
            m_OnCrayonEnteredAnimation.DOComplete();
            m_OnCrayonEnteredAnimation.DOPlayBackwards();
        }

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            foreach (var snapPoint in snapPoints)
            {
                CrayonSnapPoint crayonSnapPoint = snapPoint as CrayonSnapPoint;

                if (crayonSnapPoint.CrayonColor == (collectible as Crayon).CrayonColor &&
                    !crayonSnapPoint.IsOccupied)
                {
                    collectible.transform.parent = snapPoint.transform;
                    collectible.transform.localPosition = Vector3.zero;
                    collectible.transform.localRotation = Quaternion.identity;
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                }
                else
                {
                    m_OnCrayonEnteredAnimation.DOComplete();
                    m_OnCrayonEnteredAnimation.DOPlayBackwards();
                }
            }
        }
    }
}
