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

        private void OnTriggerEnter(Collider other)
        {
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

        private void OnTriggerExit(Collider other)
        {
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
            Gamemanager.Instance.RightAnswer();
        }

        protected override void OnPlacedInCorrectly()
        {
            Gamemanager.Instance.WrongAnswer();
        }


    }
}
