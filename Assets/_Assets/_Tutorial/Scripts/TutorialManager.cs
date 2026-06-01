using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;


namespace TMKOC.StarLink
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance;

        public List<TutorialData> tutorialDatas;


        [Header("Highlight")]
        public TutorialHighlighter highlighter;

        [Header("UI")]
        public GameObject tutorialPanel;


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
        public void StartTutorial(string tutorialId)
        {
            TutorialData tutorialData = Array.Find<TutorialData>(
          tutorialDatas.ToArray(),
          data => data.tutorialId == tutorialId
      );
            if (!CanPlayTutorial(tutorialData))
                return;

            currentTutorial = tutorialData;
            currentStepIndex = 0;

            tutorialPanel.SetActive(true);
            PlayStep();
            OnTutorialStarted?.Invoke(tutorialData.tutorialId);

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

        }

        public void PlayStep()
        {
            TutorialStep step = currentTutorial.steps[currentStepIndex];
            step.OnTutorialStepStarted?.Invoke();
            TutorialStepUI stepUI = step.stepUI;
            currentStepGO = Instantiate(step.stepUI.gameObject, tutorialPanel.transform);
            stepUI.GetComponent<TutorialStepUI>().tutorialHighlightClickHandler.target = step.targetObject;




            stepUI.tutorialText.text = step.message.ToString();
            // Helper.TypeWriterAnimation(stepUI.tutorialText,step.message,20f);

            PlayAudio(step.audioClipName);

            waitingForTargetClick = step.waitForClickOnTarget;

            if (!step.waitForClickOnTarget && step.autoNextDelay > 0)
            {
                StartCoroutine(AutoNext(step.autoNextDelay));
            }
        }

        public void OnHighlightedAreaClicked(GameObject clickedObject)
        {

            TutorialStep step = currentTutorial.steps[currentStepIndex];

            if (!step.clickAnywhere)
            {

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
            TutorialStep step = currentTutorial.steps[currentStepIndex];
            step.OnTutorialStepFinished?.Invoke();
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

            StarLinkGameManager.Instance.GamePlaying();


            tutorialPanel.SetActive(false);

            Destroy(currentStepGO);


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

       

         public void PlayAudio(string clipName)
        {
            if (clipName == null)
                return;

            if((StarLinkGameManager.Instance as StarLinkGameManager).isPlayingFromPlaySchoolMain)
            {
                RuntimeAudioLoader.Instance.PlayRuntimeAudio(AudioMapper.Instance.GetTutorialAudio(clipName));
            }
            
      
        }

        private IEnumerator AutoNext(float delay)
        {
            yield return new WaitForSeconds(delay);
            NextStep();
        }
    }
}