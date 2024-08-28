using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedPopup : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;
    [SerializeField] private TextMeshProUGUI m_LevelCompletedText;

    [SerializeField] private Button m_LevelCompletedButton;

    [SerializeField] private TextMeshProUGUI m_LevelCompletedButtonText;
    

    void OnEnable() 
    {
        Gamemanager.OnGameStart += OnGameStart;
        Gamemanager.OnGameLoose += OnGameLoose;
        Gamemanager.OnGameWin += OnGameWin;
        Gamemanager.OnGameCompleted += OnGameCompleted;

    }

    private void OnGameCompleted()
    {
         ResetData();
        HidePopup();
    }

    void OnDisable() 
    {
        Gamemanager.OnGameStart -= OnGameStart;
        Gamemanager.OnGameLoose -= OnGameLoose;
        Gamemanager.OnGameWin -= OnGameWin;
        Gamemanager.OnGameCompleted -= OnGameCompleted;


    }

    private void OnGameWin()
    {
       // SetData("Level Completed", Gamemanager.Instance.LoadNextLevel, "Next Level");
        //ShowPopup();
        
    }

    private void OnGameLoose()
    {
        SetData("No lives Remaning", Gamemanager.Instance.GameRestart, "Restart Level");
        ShowPopup();
    }

    private void OnGameStart()
    {
        ResetData();
        HidePopup();
    }

    void Start()
    {
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

    private void SetData(string levelCompletedText, Action levelCompletedButtonAction, string levelCompletedButtonText)
    {
        m_LevelCompletedText.text = levelCompletedText;

        m_LevelCompletedButton.onClick.RemoveAllListeners();

        // Add the listener correctly by passing a lambda that invokes the action
        m_LevelCompletedButton.onClick.AddListener(() => levelCompletedButtonAction?.Invoke());

        m_LevelCompletedButtonText.text = levelCompletedButtonText; // Assuming you want to set the button text as well

    }

    private void ResetData()
    {
        m_LevelCompletedText.text = "";
        m_LevelCompletedButton.onClick.RemoveAllListeners();
        m_LevelCompletedButtonText.text = ""; // Assuming you want to set the button text as well
    }
}
