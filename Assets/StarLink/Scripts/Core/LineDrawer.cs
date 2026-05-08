using System;
using UnityEngine;

namespace TMKOC.StarLink
{
    public class LineDrawer : SerializedSingleton<LineDrawer>
    {
        [Header("Line Renderers")]
        [SerializeField] private LineRenderer lineRendererDotted;
        [SerializeField] private LineRenderer lineRendererHighlighted;

        [Header("Dotted Line Settings")]
        [SerializeField] private float dottedLineWidth = 0.1f;
        [SerializeField] private Color dottedLineColor = Color.white;

        [Header("Highlighted Line Settings")]
        [SerializeField] private float highlightedLineWidth = 0.15f;
        [SerializeField] private Color highlightedLineColor = Color.yellow;

        protected override void Awake()
        {
            base.Awake();

            SetupLineRendererDotted();
            SetupLineRendererHighlighted();
        }

        private void OnEnable() 
        {
            StarLinkGameManager.OnGameStart += OnGameStart;    
        }

        private void OnGameStart()
        {
            ClearAllLines();
        }

        void OnDisable()
        {
            StarLinkGameManager.OnGameStart -= OnGameStart;    
            
        }

        private void SetupLineRendererDotted()
        {
            if (lineRendererDotted == null)
            {
                lineRendererDotted = gameObject.AddComponent<LineRenderer>();
            }

            lineRendererDotted.positionCount = 2;
            lineRendererDotted.useWorldSpace = true;

            lineRendererDotted.startWidth = dottedLineWidth;
            lineRendererDotted.endWidth = dottedLineWidth;

            lineRendererDotted.startColor = dottedLineColor;
            lineRendererDotted.endColor = dottedLineColor;

           // lineRendererDotted.material = new Material(Shader.Find("Sprites/Default"));

            lineRendererDotted.enabled = false;
        }

        private void SetupLineRendererHighlighted()
        {
            if (lineRendererHighlighted == null)
            {
                lineRendererHighlighted = gameObject.AddComponent<LineRenderer>();
            }

            // Keep this 0 because highlighted line will keep adding points.
            lineRendererHighlighted.positionCount = 0;
            lineRendererHighlighted.useWorldSpace = true;

            lineRendererHighlighted.startWidth = highlightedLineWidth;
            lineRendererHighlighted.endWidth = highlightedLineWidth;

            lineRendererHighlighted.startColor = highlightedLineColor;
            lineRendererHighlighted.endColor = highlightedLineColor;

            lineRendererHighlighted.material = new Material(Shader.Find("Sprites/Default"));

            lineRendererHighlighted.enabled = false;
        }

        public void DrawDottedLine(Vector3 startPoint, Vector3 endPoint)
        {
            lineRendererDotted.enabled = true;

            lineRendererDotted.positionCount = 2;
            lineRendererDotted.SetPosition(0, startPoint);
            lineRendererDotted.SetPosition(1, endPoint);
        }

        public void DrawHighlightedLine(Vector3 startPoint, Vector3 endPoint)
        {
            lineRendererHighlighted.enabled = true;

            int currentCount = lineRendererHighlighted.positionCount;

            // First line segment: add both start and end points.
            if (currentCount == 0)
            {
                lineRendererHighlighted.positionCount = 2;
                lineRendererHighlighted.SetPosition(0, startPoint);
                lineRendererHighlighted.SetPosition(1, endPoint);
                return;
            }

            // After first segment, continue the path by adding only the new end point.
            lineRendererHighlighted.positionCount = currentCount + 1;
            lineRendererHighlighted.SetPosition(currentCount, endPoint);
        }

        public void HideDottedLine()
        {
            lineRendererDotted.enabled = false;
        }

        public void HideHighlightedLine()
        {
            lineRendererHighlighted.enabled = false;
        }

        public void HideAllLines()
        {
            HideDottedLine();
            HideHighlightedLine();
        }

        public void ClearHighlightedLine()
        {
            lineRendererHighlighted.positionCount = 0;
            lineRendererHighlighted.enabled = false;
        }

        public void ClearAllLines()
        {
            lineRendererDotted.enabled = false;

            lineRendererHighlighted.positionCount = 0;
            lineRendererHighlighted.enabled = false;
        }
    }
}