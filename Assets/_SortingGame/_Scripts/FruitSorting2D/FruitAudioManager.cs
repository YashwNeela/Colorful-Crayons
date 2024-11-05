using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{

    public class FruitAudioManager : AudioManager
    {
        protected override void OnEnable()
        {
            Gamemanager.OnFirstTimeGameStartAction += OnFirstTimeStartGame;
            Gamemanager.OnRightAnswerAction += OnCorrectAnswer;
        }

        protected override void OnDisable()
        {
            Gamemanager.OnFirstTimeGameStartAction -= OnFirstTimeStartGame;
            Gamemanager.OnRightAnswerAction -= OnCorrectAnswer;
        }

        private void OnCorrectAnswer()
        {
            Debug.Log("YES_CORRECT");
        }
        private void OnInCorrectAnswer()
        {
            Debug.Log("NO_INCORRECT");
        }

        private void OnFirstTimeStartGame()
        {
            Debug.Log("CHK========FirstTimeStartGame");
        }
    }

}