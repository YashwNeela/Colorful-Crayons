using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC{
public class TutorialTriggerer : MonoBehaviour
{
    [SerializeField] private int m_TutorialTriggerId;

    public int TutorialTriggererId => m_TutorialTriggerId;
}
}