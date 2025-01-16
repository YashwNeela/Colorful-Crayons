using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TMKOC
{
    public class CutoutMaskUI : Image, ICanvasRaycastFilter
    {
        public override Material materialForRendering
        {
            get
            {
                Material material = new Material(base.materialForRendering);
                material.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
                return material;
            }
        }

        // RectTransform of the cutout mask
        private RectTransform m_rectTransform;

        protected override void Awake()
        {
            base.Awake();
            m_rectTransform = GetComponent<RectTransform>();
        }

        // Implement ICanvasRaycastFilter to restrict clicks to the mask
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, eventCamera);
        }
    }
}
