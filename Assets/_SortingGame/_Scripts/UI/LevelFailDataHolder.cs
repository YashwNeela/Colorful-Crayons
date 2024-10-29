using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace TMKOC.Sorting{
public class LevelFailDataHolder : MonoBehaviour
{
    [SerializeField] private Image m_Icon;

    [SerializeField] private Image m_Status;
    [SerializeField] private Sprite correctSprite, incorrectSprite;

    public void SetData(Sprite icon, bool isCorrect)
    {
        m_Icon.sprite = icon;
        if(isCorrect){
            m_Status.sprite = correctSprite;
            m_Status.color = Color.green;
        }
        else{
            m_Status.sprite = incorrectSprite;
            m_Status.color = Color.red;
        }
    }
    
}
}
