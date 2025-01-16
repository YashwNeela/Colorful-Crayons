using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIInteractionFilter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField]
    private GraphicRaycaster[] interactableGraphicRaycasters;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    private List<RaycastResult> raycastResults;
    private bool pointerUp, pointerDown;

    [SerializeField] private Image filterImage; // The image to act as the filter

    private RectTransform filterRectTransform;

    void Start()
    {
        raycastResults = new List<RaycastResult>();
        eventSystem = EventSystem.current;
        graphicRaycaster = GetComponent<GraphicRaycaster>();

        if (filterImage == null)
        {
            Debug.LogError("Filter Image is not assigned!");
            return;
        }

        filterRectTransform = filterImage.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            // CheckUIClick();
        }
    }

    private void CheckUIInteraction(Func<GameObject, bool> callback)
    {
        if (eventSystem == null || graphicRaycaster == null)
        {
            return;
        }

        pointerEventData = new PointerEventData(eventSystem);

        if (Input.touchCount > 0)
        {
            pointerEventData.position = Input.GetTouch(0).position;
        }
        else
        {
            pointerEventData.position = Input.mousePosition;
        }

        bool withinBounds = false;

        CheckInGraphicRaycast(graphicRaycaster, (result) =>
        {
            if (result == filterImage.gameObject)
            {
                withinBounds = true;
                return true;
            }
            else
            {
                return false;
            }
        });

        if (!withinBounds) return;

        foreach (var raycaster in interactableGraphicRaycasters)
        {
            if (CheckInGraphicRaycast(raycaster, (result) =>
            {
                return callback(result);
                // Button button = result.GetComponent<Button>();
                // if (button != null && button.interactable)
                // {
                //     button.onClick.Invoke();
                //     if (button.TryGetComponent<EventTrigger>(out EventTrigger eventTrigger))
                //     {
                //         //   eventTrigger.OnPointerDown()
                //         // Find all the entries for OnPointerUp
                //         foreach (var entry in eventTrigger.triggers)
                //         {
                //             if (entry.eventID == EventTriggerType.PointerDown && pointerDown)
                //             {
                //                 pointerDown = false;

                //                 entry.callback?.Invoke(pointerEventData as BaseEventData);
                //             }
                //             if (entry.eventID == EventTriggerType.PointerUp && pointerUp)
                //             {
                //                 // Simulate the event by invoking the callback
                //                 entry.callback?.Invoke(pointerEventData as BaseEventData);
                //                 pointerUp = false;
                //             }
                //         }
                //     }
                // }
                // else
                // {
                //     return false;
                // }
            }))
            {
                break;
            }
        }
    }




    private bool CheckInGraphicRaycast(GraphicRaycaster a_graphicRaycaster, Func<GameObject, bool> callback)
    {
        bool isFound = false;
        raycastResults.Clear();
        a_graphicRaycaster.Raycast(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (callback(result.gameObject))
            {
                isFound = true;
                break;
            }
        }

        return isFound;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Ppointer down");

        CheckUIInteraction((result) =>
        {
            Button button = result.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                if (button.TryGetComponent<EventTrigger>(out EventTrigger eventTrigger))
                {
                    eventTrigger.OnPointerDown(pointerEventData);
                }
                return true;
            }
            else return false;
        });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Ppointer up");

        CheckUIInteraction((result) =>
        {
            Button button = result.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                if (button.TryGetComponent<EventTrigger>(out EventTrigger eventTrigger))
                {
                    eventTrigger.OnPointerUp(pointerEventData);
                }
                return true;
            }
            else return false;
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        CheckUIInteraction((result) =>
        {
            Button button = result.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                return true;
            }
            else return false;
        });
    }
}