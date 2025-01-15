using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC{
public class ControlsTutorialUI : MonoBehaviour
{
    public GameObject m_Container;

    public GameObject maskImage;

    public void Activate(TutorialStep step)
    {
        m_Container.SetActive(true);
        maskImage.GetComponent<RectTransform>().position = step.controlObjects.GetComponent<RectTransform>().position;
    }

    public void Deactivate()
    {
        m_Container.SetActive(false);
    }
}
}
