using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMKOC.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC
{
    public class CutSceneUI : MonoBehaviour
    {
        public TextMeshProUGUI titleText;

        public TextMeshProUGUI descriptionText;

        public Button m_NextButton;

        public Image m_Image;

        public GameObject m_Container;

        void OnEnable()
        {

           m_NextButton.onClick.AddListener(()=>
           {
                CutSceneEventManager.Instance.TriggerEvent(CinematicCutSceneManager.Instance.GetCurrentTutorialStep().eventName);
                Debug.Log("Next scene button");
                m_NextButton.gameObject.SetActive(false);
           }); 
        }

        void OnDisable()
        {
            m_NextButton.onClick.RemoveAllListeners();
            
        }

        public void ShowStep(CutSceneStep step)
        {
                m_NextButton.gameObject.SetActive(false);
            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(step.delay, () =>
            {
                OnStepShow(step);
                AudioManager.Instance.PlayAudio(step.audioClip,AudioManager.Instance.SFXAudioSource);

            }));
        }

        void OnStepShow(CutSceneStep step)
        {
            m_Container.SetActive(true);
            titleText.text = step.stepTitle;

            Helper.TypeWriterAnimation(descriptionText,step.description,20f,()=>
            {
                m_NextButton.gameObject.SetActive(true);
                m_NextButton.GetComponent<DOTweenAnimation>().DOPlay();
            });

            m_Image.sprite = step.imageSprite;
        }

        public void Hide()
        {
            m_Container.SetActive(false);
        }

        public void HandleGameobjectsVisibilityBasedOnCurrentStep(TutorialStep step)
        {
            foreach (string controlId in step.enableControlsId)
            {
                ControlsUI.Instance.EnableControls(controlId);
            }

            foreach (string controlId in step.disableControlsId)
            {
                ControlsUI.Instance.DisableControls(controlId);
            }
        }
    }
}
