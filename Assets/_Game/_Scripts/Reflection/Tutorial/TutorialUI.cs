using System;
using DG.Tweening;
using TMKOC.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC
{

    public class TutorialUI : MonoBehaviour
    {
        [Header("World Elements")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;

        public Button m_NextButton;

        public Image m_Image;

        public GameObject m_WorldElementsContainer;

        [Header("Controls Elements")]
        [Space(10)]
        public ControlsTutorialUI m_ControlsContainer;

        void OnEnable()
        {
            m_NextButton.onClick.AddListener(() =>
            {

                TutorialEventManager.Instance.TriggerEvent(TutorialManager.Instance.GetCurrentTutorialStep().eventName);
                m_NextButton.gameObject.SetActive(false);
            });
        }

        void OnDisable()
        {
            m_NextButton.onClick.RemoveAllListeners();
        }

        public void ShowStep(TutorialStep step)
        {
            HandleGameobjectsVisibilityBasedOnCurrentStep(step);

            StartCoroutine(StaticCoroutine.Co_GenericCoroutine(step.delay, () =>
            {
                switch (step.type)
                {
                    case TutorialType.Controls:
                        ControlsStep(step);
                        break;
                    case TutorialType.World:
                        WorldElementStep(step);
                        break;
                }
            }));

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

        public void WorldElementStep(TutorialStep step)
        {
            m_ControlsContainer.Deactivate();
            m_WorldElementsContainer.SetActive(true);

            titleText.text = step.stepTitle;
            // descriptionText.text = step.description;
            m_Image.GetComponent<DOTweenAnimation>().DORewind();

            m_Image.GetComponent<DOTweenAnimation>().DOPlay();
            Helper.TypeWriterAnimation(descriptionText, step.description, 20f, () =>
            {
                m_NextButton.gameObject.SetActive(true);
                m_NextButton.GetComponent<DOTweenAnimation>().DOPlay();
            });


            m_Image.sprite = step.imageSprite;
        }

        public void ControlsStep(TutorialStep step)
        {
            m_WorldElementsContainer.SetActive(false);
            m_ControlsContainer.Activate(step);
        }

        public void Hide()
        {
            m_WorldElementsContainer.SetActive(false);
            m_ControlsContainer.Deactivate();
        }






    }


}
