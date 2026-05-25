using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace TMKOC.StarLink
{
    public class TutorialHighlightClickHandler : MonoBehaviour
    {
        // public TutorialHighlighter highlighter;

        public GameObject target;

        public void OnHighlightedAreaClicked()
        {
            TutorialManager.Instance.OnHighlightedAreaClicked(target);

        }

        public void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(IsPointerOverTarget())
                    OnHighlightedAreaClicked();

            }
        }

        bool IsPointerOverTarget()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.layer == LayerMask.NameToLayer("UI"))
                {
                if(result.gameObject == target)
                    return true;
                }
                

            }

            return false;
        }
    }

    
    }


 