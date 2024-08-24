using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMKOC.Colorful_Crayons
{
    public enum CrayonColor
    {
        Red,
        Yellow,

        Green,

        Blue
    }

    public class CrayonBox : Collector
    {
        [SerializeField] private Color m_BoxColor;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;

        private Renderer m_Renderer;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            SetBoxColor(m_BoxColor);
        }

        private void SetBoxColor(Color color)
        {
            m_Renderer.material.color = color;
        }
    }


}
