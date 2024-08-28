using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMKOC.Sorting;
using UnityEngine.Windows;


namespace TMKOC.Sorting.ColorfulCrayons
{
    [Flags]
    public enum CrayonColor
    {
        None = 0,
        Red = 1<<0,
        Yellow = 1<<1,

        Green = 1<<2,

        Blue = 1<<3
    }

    public class CrayonBox : Collector
    {
        [SerializeField] private Color m_BoxColor1, m_BoxColor2;
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
            SetBoxColor(m_BoxColor1, m_BoxColor2);
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



        private void SetBoxColor(Color color1, Color color2)
        {
            m_Renderer.materials[0].color = color1;
            m_Renderer.materials[1].color = color2;


        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>

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
            //m_OnCrayonEnteredAnimation.autoGenerate = true;
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
                    
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                
                }else
                {
                    m_OnCrayonEnteredAnimation.DOComplete();
                    m_OnCrayonEnteredAnimation.DOPlayBackwards(); 
                }
            }
        }


    }


}
