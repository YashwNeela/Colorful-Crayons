using UnityEngine;

namespace TMKOC.Colorful_Crayons
{
    public class Crayon : MonoBehaviour
    {
        [SerializeField] private Color m_Color;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;
        public Color Color => m_Color;

        private Renderer m_Renderer;
        private CrayonBox currentBox;

        void Awake()
        {
            m_Renderer = GetComponent<Renderer>();
            SetBoxColor(m_Color);

            // Subscribe to the OnDragEnd event
            var draggable = GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.OnDragEnd += HandleDragEnd;
            }
        }

        private void SetBoxColor(Color color)
        {
            m_Renderer.material.color = color;
        }

        private void OnTriggerEnter(Collider other)
        {
            CrayonBox crayonBox = other.GetComponent<CrayonBox>();
            if (crayonBox != null && crayonBox.CrayonColor == this.CrayonColor)
            {
                currentBox = crayonBox;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            CrayonBox crayonBox = other.GetComponent<CrayonBox>();
            if (crayonBox != null && crayonBox == currentBox)
            {
                currentBox = null;
            }
        }

        private void HandleDragEnd()
        {
            if (currentBox != null)
            {
                currentBox.SnapCrayonToBox(this);
            }
        }
    }
}
