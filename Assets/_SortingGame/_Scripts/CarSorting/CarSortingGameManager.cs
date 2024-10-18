using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting.CarSorting{
public class CarSortingGameManager : Gamemanager
{
    [SerializeField] private Image m_Background;

    public void ChangeBackground(Sprite image)
    {
        m_Background.sprite = image;
    }
    
}
}
