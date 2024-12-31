using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TMKOC
{
    public class Level : MonoBehaviour
    {
        public string m_Tip;

        [SerializeField]
        public UnityEvent onLevelCompleteCheck;
        protected virtual void Awake()
        {

        }

        protected virtual void OnEnable()
        {
            GameManager.OnGameStart += OnGameStart;
            GameManager.OnGameRestart += OnGameRestart;
            GameManager.OnGameWin += OnGameWin;
            GameManager.OnLevelCompleteCheck += OnLevelCompleteCheck;
        }

        protected virtual void OnDisable()
        {
            GameManager.OnGameStart -= OnGameStart;

            GameManager.OnGameRestart -= OnGameRestart;
            GameManager.OnGameWin -= OnGameWin;

            GameManager.OnLevelCompleteCheck -= OnLevelCompleteCheck;
        }

        protected virtual void OnGameWin()
        {

        }
        protected virtual void OnGameStart()
        {

        }

        protected virtual void OnGameRestart()
        {

        }

        protected virtual void OnLevelCompleteCheck()
        {
            onLevelCompleteCheck?.Invoke();

        }

        public virtual void OnLevelLoaded(){}

        public virtual void OnLevelUnloaded(){}
    }
}
