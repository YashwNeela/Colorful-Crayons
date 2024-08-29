using UnityEngine;
using TMKOC.Sorting;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class Crayon : Collectible
    {
        [SerializeField] private Color m_Color;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;
        public Color Color => m_Color;

        private Renderer m_Renderer;
        [SerializeField] private int m_MaterialIndex;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            SetBoxColor(m_CrayonColor);


        }

        private void SetBoxColor(CrayonColor crayonColor)
        {
            if (crayonColor.HasFlag(CrayonColor.Red))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.red;
                
            }
            if (crayonColor.HasFlag(CrayonColor.Yellow))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.yellow;

               
            }
            if (crayonColor.HasFlag(CrayonColor.Green))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.green;
                
            }
            if (crayonColor.HasFlag(CrayonColor.Blue))
            {
                m_Renderer.materials[m_MaterialIndex].color = Color.blue;
               
            }
            
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            CrayonBox collectorBox = other.GetComponent<Collector>() as CrayonBox;
            if (collectorBox != null)
            {
                if (collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                {
                    m_ValidCollector = collectorBox;
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
                if(collectorBox == m_ValidCollector)
                    m_ValidCollector = null;

                m_IsTryingToPlaceWrong = false;
               
            }
        }

        

        
        protected override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);
            CrayonBox collectorBox = other.GetComponent<Collector>() as CrayonBox;
            if (collectorBox != null)
            {
                if (!collectorBox.CrayonColor.HasFlag(this.CrayonColor))
                    m_IsTryingToPlaceWrong = true;

            }
        }

        protected override void OnPlacedCorrectly()
        {
            base.OnPlacedCorrectly();
            Gamemanager.Instance.RightAnswer();
        }

        protected override void PlaceInCorrectly(Collector collector)
        {
            base.PlaceInCorrectly(collector);
            Gamemanager.Instance.WrongAnswer();
        }

        


    }
}
