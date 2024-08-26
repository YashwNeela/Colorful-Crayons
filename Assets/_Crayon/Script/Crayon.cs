using UnityEngine;

namespace TMKOC.Colorful_Crayons
{
    public class Crayon : Collectible
    {
        [SerializeField] private Color m_Color;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;
        public Color Color => m_Color;

        private Renderer m_Renderer;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            SetBoxColor(m_Color);


        }

        private void SetBoxColor(Color color)
        {
            m_Renderer.material.color = color;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            CrayonBox collectorBox = other.GetComponent<Collector>() as CrayonBox;
            if (collectorBox != null)
            {
                if (collectorBox.CrayonColor == this.CrayonColor)
                {
                    currentCollector = collectorBox;
                }
                else
                    m_IsTryingToPlaceWrong = true;
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            CrayonBox collectorBox = other.GetComponent<Collector>() as CrayonBox;
            if (collectorBox != null)
            {
                if(collectorBox == currentCollector)
                    currentCollector = null;

                m_IsTryingToPlaceWrong = false;
               
            }
        }

        
        void OnTriggerStay(Collider other)
        {
            
        }

        protected override void OnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
            Gamemanager.Instance.RightAnswer();
        }

        protected override void PlaceInCorrectly()
        {
            base.PlaceInCorrectly();
            Gamemanager.Instance.WrongAnswer();
        }

        


    }
}
