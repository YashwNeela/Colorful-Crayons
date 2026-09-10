using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace TMKOC.Cases_of_Popatlal.Tutorial{

public class PassThroughRaycastHandler : MonoBehaviour,IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. Handle click on mask itself
        Debug.Log("Mask clicked");

        // 2. Raycast all UI objects under pointer
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            // Skip self
           // if (result.gameObject == gameObject)
               // continue;

            // Forward click to objects behind
            ExecuteEvents.Execute(
                result.gameObject,
                eventData,
                ExecuteEvents.pointerClickHandler
            );

            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
            }


            // break if you only want first object behind
         
        }
    }
}


}