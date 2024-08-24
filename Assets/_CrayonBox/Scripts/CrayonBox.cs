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

    public class CrayonBox : MonoBehaviour
    {
        [SerializeField] private Color m_BoxColor;
        [SerializeField] private CrayonColor m_CrayonColor;

        public CrayonColor CrayonColor => m_CrayonColor;

        private Renderer m_Renderer;
        private SnapPoint[] snapPoints;

        void Awake()
        {
            m_Renderer = GetComponent<Renderer>();
            SetBoxColor(m_BoxColor);

            // Get all the SnapPoint components attached to child objects
            snapPoints = GetComponentsInChildren<SnapPoint>();
        }

        private void SetBoxColor(Color color)
        {
            m_Renderer.material.color = color;
        }
        public void SnapCrayonToBox(Crayon crayon)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    crayon.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    crayon.transform.parent = snapPoint.transform; // Change parent first
                    crayon.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    crayon.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    break;
                }
            }
        }
    }


}
