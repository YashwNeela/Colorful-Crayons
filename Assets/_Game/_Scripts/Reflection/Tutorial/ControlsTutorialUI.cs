using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC
{
    public class ControlsTutorialUI : MonoBehaviour
    {
        public GameObject m_Container;

        public GameObject maskImage;

        public GameObject m_DescriptionContainer;

        public TextMeshProUGUI description;

        public GameObject[] descriptionAnchorPoints;

        Vector2 screenCenter;

        void Awake()
        {
            screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        }

        public void Activate(TutorialStep step)
        {
            m_Container.SetActive(true);
            RectTransform rectTransform = step.controlObjects.GetComponent<RectTransform>();
            RectTransform maskRect = maskImage.GetComponent<RectTransform>();

            // Copy size and position
            maskRect.sizeDelta = rectTransform.sizeDelta;
            

            // Copy anchor points
            maskRect.anchorMin = rectTransform.anchorMin;
            maskRect.anchorMax = rectTransform.anchorMax;

            // Copy pivot (optional, if you want it to match exactly)
            maskRect.pivot = rectTransform.pivot;
            maskRect.position = rectTransform.position;

            var pos = (Vector2)maskImage.GetComponent<RectTransform>().position - screenCenter;

            //Top Left
            if (pos.x < 0 && pos.y > 0)
            {
                Debug.Log("Top Left");
                m_DescriptionContainer.transform.parent = descriptionAnchorPoints[2].transform;
                m_DescriptionContainer.GetComponent<RectTransform>().localPosition = Vector3.zero;

            }

            //Top Right
            if (pos.x > 0 && pos.y > 0)
            {
                Debug.Log("Top Right");
                m_DescriptionContainer.transform.parent = descriptionAnchorPoints[3].transform;
                m_DescriptionContainer.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }

            //Bottom Right
            if (pos.x > 0 && pos.y < 0)
            {
                Debug.Log("Bottom Right");
                m_DescriptionContainer.transform.parent = descriptionAnchorPoints[0].transform;
                m_DescriptionContainer.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }

            //Bottom Left
            if (pos.x < 0 && pos.y < 0)
            {
                Debug.Log("Bottom Left");
                m_DescriptionContainer.transform.parent = descriptionAnchorPoints[1].transform;
                m_DescriptionContainer.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }

            Helper.TypeWriterAnimation(description, step.description, 20f);

        }



        public void Deactivate()
        {
            m_Container.SetActive(false);
        }
    }
}
