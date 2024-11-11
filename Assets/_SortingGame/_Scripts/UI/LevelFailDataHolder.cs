using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class LevelFailDataHolder : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;

        [SerializeField] private Image m_Status;
        [SerializeField] private Sprite correctSprite, incorrectSprite, questionMark;

        public void SetData(Sprite icon,Color iconColor, bool isCorrect, bool isempty = false)
        {
            m_Icon.sprite = icon;
            m_Icon.color = iconColor;

            if (!isempty)
            {
                m_Status.gameObject.SetActive(true);

                if (isCorrect)
                {
                    m_Status.sprite = correctSprite;
                    m_Status.color = Color.green;
                }
                else
                {
                    m_Status.sprite = incorrectSprite;
                    m_Status.color = Color.red;
                }
            }
            else
            {
                m_Icon.sprite = questionMark;
                m_Icon.color = Color.red;
                m_Status.gameObject.SetActive(false);
            }

        }

    }
}
