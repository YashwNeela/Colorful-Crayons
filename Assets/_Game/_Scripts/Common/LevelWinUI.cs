using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting{
public class LevelWinUI : MonoBehaviour
{
    [SerializeField] private Button m_NextLevelButton;
    void OnEnable()
    {
        m_NextLevelButton.interactable = false;
        m_NextLevelButton.onClick.RemoveAllListeners();
        m_NextLevelButton.onClick.AddListener(()=> { m_NextLevelButton.interactable = false; });
        
        StartCoroutine(StaticCoroutine.Co_GenericCoroutine(3,()=>
        {
            m_NextLevelButton.interactable = true;
        }));
    }
}
}
