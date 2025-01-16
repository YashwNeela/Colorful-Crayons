using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC{


public class TutorialManager : SerializedSingleton<TutorialManager>
{
    public List<TutorialStep> tutorialSteps; // Steps in the tutorial
    private int currentStepIndex = 0;

    public TutorialUI tutorialUI;  // Reference to the UI manager
    public Camera mainCamera;      // Main camera for focusing objects

    public bool m_IsTutorialActive;

    public bool IsTutorialActive => m_IsTutorialActive;
    

    private void Start()
    {
        StartTutorial();
    }


    public void StartTutorial()
    {
        if (tutorialSteps.Count == 0)
        {
            Debug.LogWarning("No tutorial steps found!");
            m_IsTutorialActive = false;
            return;
        }
            m_IsTutorialActive = true;

        ShowStep(currentStepIndex);
    }

    private void ShowStep(int index)
    {
        if (index >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }

        var step = tutorialSteps[index];
        tutorialUI.ShowStep(step);

        if (step.imageSprite != null)
        {
            //HighlightObject(step.highlightObject);
        }

        if (step.cameraFocusPosition != Vector3.zero)
        {
            FocusCamera(step.cameraFocusPosition);
        }

        if (step.requiresEvent)
        {
            TutorialEventManager.Instance.Subscribe(step.eventName, OnStepEventTriggered);
        }
        else
        {
            StartCoroutine(AutoAdvanceStep(step.duration));
        }
    }

    private IEnumerator AutoAdvanceStep(float duration)
    {
        yield return new WaitForSeconds(duration);
        NextStep();
    }

    private void OnStepEventTriggered()
    {
        TutorialEventManager.Instance.Unsubscribe(tutorialSteps[currentStepIndex].eventName, OnStepEventTriggered);
        NextStep();
    }

    private void NextStep()
    {
        currentStepIndex++;
        StartCoroutine(StaticCoroutine.Co_GenericCoroutine(tutorialSteps[currentStepIndex].delay,()=>
        {
            ShowStep(currentStepIndex);
        }));

    }

    private void HighlightObject(GameObject obj)
    {
        // Implement object highlighting logic here (e.g., using a shader or outline effect)
    }

    private void FocusCamera(Vector3 position)
    {
        mainCamera.transform.position = position; // Add smoothing if needed
    }

    private void EndTutorial()
    {
        Debug.Log("Tutorial Complete!");
        tutorialUI.Hide();
        m_IsTutorialActive = false;

    }
}
}