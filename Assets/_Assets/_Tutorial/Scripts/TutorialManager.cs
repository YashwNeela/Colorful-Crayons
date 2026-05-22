using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Sirenix.OdinInspector;
using System.Linq;
using TMKOC.StarLink;
using TMKOC.Cases_of_Popatlal.Tutorial;
using System;



public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;


    [Header("Highlight")]
    public TutorialHighlighter highlighter;

    [Header("UI")]
    public GameObject tutorialPanel;

    [Header("Audio")]
    public AudioSource audioSource;

    private TutorialData currentTutorial;

    private GameObject currentStepGO;
    private int currentStepIndex;
    private bool waitingForTargetClick;

    public Action<string> OnTutorialStarted;
    public Action<string> OnTutorialFinished;

    

    private void Awake()
    {
        Instance = this;
        tutorialPanel.SetActive(false);
    }

    [Button]
    public void StartTutorial(TutorialData tutorialData)
    {
        if (!CanPlayTutorial(tutorialData))
            return;

        currentTutorial = tutorialData;
        currentStepIndex = 0;

        tutorialPanel.SetActive(true);
        PlayStep();
        OnTutorialStarted?.Invoke(tutorialData.tutorialId);
        StarLinkGameManager.Instance.StartTutorial(tutorialData.tutorialId);
    }

    public void PlayStep()
    {
        TutorialStep step = currentTutorial.steps[currentStepIndex];
        TutorialStepUI stepUI = step.stepUI;
        currentStepGO =  Instantiate(step.stepUI.gameObject,tutorialPanel.transform);
       // stepUIGO.GetComponent<TutorialStepUI>().tutorialHighlightClickHandler.target = step.targetObject;

        
    

        stepUI.tutorialText.text = step.message.ToString();
       // Helper.TypeWriterAnimation(stepUI.tutorialText,step.message,20f);
       
        PlayAudio(step.audioClip);

        waitingForTargetClick = step.waitForClickOnTarget;

        if (!step.waitForClickOnTarget && step.autoNextDelay > 0)
        {
            StartCoroutine(AutoNext(step.autoNextDelay));
        }
    }

    public void OnHighlightedAreaClicked(GameObject clickedObject)
    {
     
        TutorialStep step = currentTutorial.steps[currentStepIndex];

        if(!step.clickAnywhere){

        if (!waitingForTargetClick)
            return;

        if (clickedObject == step.targetObject)
        {
            NextStep();
        }

        }
        else
            NextStep();

        
    }

    public void NextStep()
    {
        Destroy(currentStepGO);
        currentStepIndex++;

        if (currentStepIndex >= currentTutorial.steps.Length)
        {
            CompleteTutorial();
            return;
        }

        PlayStep();
    }

    public void CompleteTutorial()
    {
        SaveTutorialCompleted(currentTutorial);

        OnTutorialFinished?.Invoke(currentTutorial.tutorialId);
        StarLinkGameManager.Instance.EndTutorial(currentTutorial.tutorialId);


        tutorialPanel.SetActive(false);

        currentTutorial = null;
    }

    public bool CanPlayTutorial(TutorialData tutorialData)
    {
        if (tutorialData.playMode == TutorialPlayMode.PlayEveryTime)
            return true;

        return PlayerPrefs.GetInt(tutorialData.tutorialId, 0) == 0;
    }

    public void SaveTutorialCompleted(TutorialData tutorialData)
    {
        if (tutorialData.playMode == TutorialPlayMode.PlayOnce ||
            tutorialData.playMode == TutorialPlayMode.PlayUntilCompleted)
        {
            PlayerPrefs.SetInt(tutorialData.tutorialId, 1);
            PlayerPrefs.Save();
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if (clip == null)
            return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    private IEnumerator AutoNext(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextStep();
    }
}
