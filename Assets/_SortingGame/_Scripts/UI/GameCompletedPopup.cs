using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace TMKOC.Sorting
{

    public class GameCompletedPopup : MonoBehaviour
    {
        [SerializeField] private GameObject m_Container;

        [SerializeField] private TextMeshProUGUI m_GameCompletedText;

        [SerializeField] private Button m_GameCompletedButton;

        [SerializeField] private TextMeshProUGUI m_GameCompletedButtonText;

        void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameCompleted += OnGameCompleted;
           

        }

        void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;
            Gamemanager.OnGameCompleted -= OnGameCompleted;
        }

        private void OnGameCompleted()
        {
            SetData("GameCompleted", ()=> {}, "GoBack");
            ShowPopup();
        }

        private void OnGameStart()
        {
            ResetData();
           HidePopup();

        }

        private void ShowPopup()
        {
            m_Container.SetActive(true);
        }

        private void HidePopup()
        {
            m_Container.SetActive(false);
        }

        private void SetData(string gameCompletedText, Action gameCompletedButtonAction, string gameCompletedButtonText)
        {
            m_GameCompletedText.text = gameCompletedText;

            m_GameCompletedButton.onClick.RemoveAllListeners();

            // Add the listener correctly by passing a lambda that invokes the action
            m_GameCompletedButton.onClick.AddListener(() => gameCompletedButtonAction?.Invoke());

            m_GameCompletedButtonText.text = gameCompletedButtonText; // Assuming you want to set the button text as well

        }

        private void ResetData()
        {
            m_GameCompletedText.text = "";
            m_GameCompletedButton.onClick.RemoveAllListeners();
            m_GameCompletedButtonText.text = ""; // Assuming you want to set the button text as well
        }

        public void OnHomeButtonClicked()
        {
            Gamemanager.Instance.GoBackToPlayschool();
        }
        public void OnRetry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
