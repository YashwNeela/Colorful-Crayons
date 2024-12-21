using UnityEngine;
using TMKOC.Sorting;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class Crayon : Collectible
    {
        [SerializeField] protected Color m_Color;
        [SerializeField] protected CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;
        public Color Color => m_Color;

        protected Renderer m_Renderer;
        [SerializeField] protected int m_MaterialIndex;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            SetCrayonColor(m_CrayonColor);


        }

        protected virtual void SetCrayonColor(CrayonColor crayonColor)
        {
            if (crayonColor.HasFlag(CrayonColor.CrayonRed))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.red;

            }
            if (crayonColor.HasFlag(CrayonColor.CrayonYellow))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.yellow;


            }
            if (crayonColor.HasFlag(CrayonColor.CrayonGreen))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.green;

            }
            if (crayonColor.HasFlag(CrayonColor.CrayonBlue))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.blue;

            }

        }

        protected override void HandleDragStart()
        {
            base.HandleDragStart();
            CrayonSortingAudioManager Instance = (CrayonSortingAudioManager.Instance as CrayonSortingAudioManager);
            Instance.PlayColorNameAudio(m_CrayonColor);
        }

        protected override void HandleCollectorOnTriggerEnter(Component collider)
        {
            base.HandleCollectorOnTriggerEnter(collider);
            CrayonBox collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as CrayonBox;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as CrayonBox;

            if (collectorBox != null)
            {
                // if (collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                // {
                //     m_ValidCollector = collectorBox;
                // }
                // else
                //     m_IsTryingToPlaceWrong = true;
                m_ValidCollector = collectorBox;
            }

        }

        protected virtual void BaseHandleCollectorOnTriggerExit(Component collider)
        {
            base.HandleCollectorOnTriggerExit(collider);
        }

        protected override void HandleCollectorOnTriggerExit(Component collider)
        {
            base.HandleCollectorOnTriggerExit(collider);
            CrayonBox collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as CrayonBox;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as CrayonBox;

            if (collectorBox != null)
            {
                if (collectorBox == m_ValidCollector)
                    m_ValidCollector = null;

                m_IsTryingToPlaceWrong = false;

            }
        }

        protected override void HandleCollectorOnTriggerStay(Component collider)
        {
            base.HandleCollectorOnTriggerStay(collider);
            CrayonBox collectorBox = null;
            if (collider is Collider)
                collectorBox = ((Collider)collider).GetComponent<Collector>() as CrayonBox;
            else if (collider is Collider2D)
                collectorBox = ((Collider2D)collider).GetComponent<Collector>() as CrayonBox;

            if (collectorBox != null)
            {
                if (!collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                    m_IsTryingToPlaceWrong = true;

            }
        }

        protected override void OnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
            (SortingGameManager.Instance as SortingGameManager).RightAnswer();
        }

        protected virtual void BaseOnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
        }

        protected override void PlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);
            (SortingGameManager.Instance as SortingGameManager).WrongAnswer();
        }

        protected virtual void BaseOnPlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);
        }



    }
}
