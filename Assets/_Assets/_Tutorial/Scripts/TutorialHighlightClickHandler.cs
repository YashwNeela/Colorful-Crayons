using UnityEngine;
using UnityEngine.EventSystems;



public class TutorialHighlightClickHandler : MonoBehaviour, IPointerDownHandler
{
   // public TutorialHighlighter highlighter;

   public GameObject target;

    public void OnClick()
        {
        Debug.Log("On Button click");

            TutorialManager.Instance.OnHighlightedAreaClicked(target);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
             Debug.Log("On pointer click");
        if (target != null)
        {
            TutorialManager.Instance.OnHighlightedAreaClicked(target);
        }
        }
    }
