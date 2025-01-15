using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TMKOC{
public class DisableTouchExceptSpecificUI : MonoBehaviour
{
    public GameObject targetUIElement; // The specific UI element to allow interaction behind
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Start()
    {
        // Get the raycaster and event system components
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (raycaster == null)
        {
            Debug.LogError("GraphicRaycaster is required on the Canvas.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // For touches, you can use Input.touchCount > 0
        {
            // Check if the specific UI element is under the pointer
            if (IsPointerOverTargetUI())
            {
                Debug.Log("Pointer is over the target UI element!");
                // Allow interaction with other UI behind this element
            }
            else
            {
                Debug.Log("Pointer is not over the target UI element!");
                // Disable interactions elsewhere
            }
        }
    }

    private bool IsPointerOverTargetUI()
    {
        // Create a pointer event
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        // Raycast using the GraphicRaycaster
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // Check if the specific UI element is among the raycast results
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetUIElement)
            {
                return true;
            }
        }

        return false;
    }
}
}