using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMKOC.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC
{
    public class CutSceneData
    {
        public int cutSceneId;

        public bool shouldEnableControlsUIOnFinished;

        [ShowIf(nameof(shouldEnableControlsUIOnFinished))]
        public List<string> enableControlsId;


        public List<CutSceneStep> cutSceneSteps;

        [SerializeField] public UnityAction OnCutSceneEndAction;

    }

    public class CutSceneStep
    {
        public string stepTitle;          // Title of the step
        [TextArea] public string description; // Description or instructions
        public GameObject controlObjects; // Object to highlight (optional)

        public Sprite imageSprite;


        public List<string> enableControlsId;

        public List<string> disableControlsId;

        public AudioClip audioClip;

        public float duration = 5f;       // Duration before auto-progressing (if applicable)

        public float delay = 0;

        public bool requiresEvent;        // Does the step wait for an event?
        public string eventName;          // Name of the event to wait for (optional)


    }
    public class CinematicCutSceneManager : SerializedSingleton<CinematicCutSceneManager>
    {
        public Action<int> OnCinematicCutSceneStarted;

        public Action<int> OnCinematicCutSceneEnded;

        public List<CutSceneData> cutSceneDatas;

        protected CutSceneData m_CurrentCutSceneData;

        public CutSceneData CurrentCutSceneData => m_CurrentCutSceneData;

        [SerializeField] protected CutSceneUI m_CutSceneUI;

        public CutSceneUI CutSceneUI => m_CutSceneUI;

        private int currentStepIndex = 0;

        private bool m_IsCutSceneActive = false;

        public bool IsCutSceneActive => m_IsCutSceneActive;

        protected Action CurrentCutSceneFinishedCallback;



        public CutSceneStep GetCurrentTutorialStep()
        {
            return CurrentCutSceneData.cutSceneSteps[currentStepIndex];
        }
        public void StartCutScene(int cutSceneId, Action OnCutSceneFinishedCallback = null)
        {
            m_CurrentCutSceneData = cutSceneDatas.Find(data => data.cutSceneId == cutSceneId);

            if (m_CurrentCutSceneData.cutSceneSteps.Count == 0)
            {
                Debug.LogWarning("No cutScene steps found!");
                return;
            }

            CurrentCutSceneFinishedCallback = OnCutSceneFinishedCallback;
            m_IsCutSceneActive = true;
            ShowStep(currentStepIndex);

            OnCinematicCutSceneStarted?.Invoke(cutSceneId);
        }

        public void ShowStep(int index)
        {
            if (index >= CurrentCutSceneData.cutSceneSteps.Count)
            {
                EndCutScene();
                return;
            }
            var step = CurrentCutSceneData.cutSceneSteps[index];

            //Enable CutScene Here
            m_CutSceneUI.ShowStep(step);

            if (step.requiresEvent)
            {
                CutSceneEventManager.Instance.Subscribe(step.eventName, OnStepEventTriggered);
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
            CutSceneEventManager.Instance.Unsubscribe(CurrentCutSceneData.cutSceneSteps[currentStepIndex].eventName, OnStepEventTriggered);
            NextStep();
        }

        private void NextStep()
        {
            currentStepIndex++;

            //Hide Previous CutScene Data here

            if (currentStepIndex >= CurrentCutSceneData.cutSceneSteps.Count)
            {
                EndCutScene();
                return;
            }

            ShowStep(currentStepIndex);
        }

        public void EndCutScene()
        {
            OnCinematicCutSceneEnded?.Invoke(CurrentCutSceneData.cutSceneId);
            if(m_CurrentCutSceneData.OnCutSceneEndAction != null)
                m_CurrentCutSceneData?.OnCutSceneEndAction();
            if (m_CurrentCutSceneData.shouldEnableControlsUIOnFinished)
            {
                foreach (string controlId in m_CurrentCutSceneData.enableControlsId)
                {
                    ControlsUI.Instance.EnableControls(controlId);
                }
            }
            m_CurrentCutSceneData = null;
            currentStepIndex = 0;
            Debug.Log("CutScene Ended");
            m_IsCutSceneActive = false;
            //Hide all cutscene GameObjects Here

            CurrentCutSceneFinishedCallback?.Invoke();
            CurrentCutSceneFinishedCallback = null;
            m_CutSceneUI.Hide();

        }



    }


}
