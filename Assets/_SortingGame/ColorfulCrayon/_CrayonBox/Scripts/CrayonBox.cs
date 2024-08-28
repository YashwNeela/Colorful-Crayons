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
        Red = 1<<0,
        Yellow = 1<<1,

        Green = 1<<2,

        Blue = 1<<3
    }

    public class CrayonBox : Collector
    {
        [SerializeField] private Color m_BoxColor;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        [SerializeField] private DOTweenAnimation m_OnCrayonEnteredAnimation;

    

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            SetBoxColor(m_BoxColor);
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



        private void SetBoxColor(Color color)
        {
            m_Renderer.material.color = color;
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

      
    }


}
