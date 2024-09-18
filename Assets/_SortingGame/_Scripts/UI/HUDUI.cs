using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class HUDUI : MonoBehaviour
    {
        public void OnBackButtonClicked()
        {
            Gamemanager.Instance.GoBackToPlayschool();
        }

        public void LevelCompleteCheck()
        {
            Gamemanager.Instance.LevelCompleteCheck();
        }
    }
}
