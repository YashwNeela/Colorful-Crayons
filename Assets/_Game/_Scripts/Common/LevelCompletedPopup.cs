using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using TMKOC.Sorting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC{
public class LevelCompletedPopup : MonoBehaviour
{
    [SerializeField] protected GameObject m_LooseContainer, m_WinContainer;
    [SerializeField] protected TextMeshProUGUI m_LevelCompletedText;

    [SerializeField] protected Button m_LevelCompletedButton;

    [SerializeField] protected TextMeshProUGUI m_LevelCompletedButtonText;

    [SerializeField] protected GameObject[] m_WinText;

    [SerializeField] protected LevelFailUI m_LevelFailUI;


    protected virtual void Awake()
    {
       
    }
    protected virtual void OnEnable()
    {
        GameManager.OnGameOver += OnGameOver;
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnGameLoose += OnGameLoose;
        GameManager.OnGameWin += OnGameWin;
        GameManager.OnGameCompleted += OnGameCompleted;

    }

   

    private void OnGameCompleted()
    {
        ResetData();
        HidePopup();
    }

    protected virtual void OnDisable()
    {
        GameManager.OnGameOver -= OnGameOver;

        GameManager.OnGameStart -= OnGameStart;
        GameManager.OnGameLoose -= OnGameLoose;
        GameManager.OnGameWin -= OnGameWin;
        GameManager.OnGameCompleted -= OnGameCompleted;


    }

    protected virtual void OnGameOver()
    {

    }

    protected virtual void OnGameWin()
    {
        // SetData("Level Completed", Gamemanager.Instance.LoadNextLevel, "Next Level");
        EnableRandomWinText();
        ShowWinPopup();

    }

    private void OnGameLoose()
    {
        SetData("Oh no! Try Again!", GameManager.Instance.GameRestart, "Restart");
        ShowLoosePopup();
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

    private void ShowLoosePopup()
    {
        m_WinContainer.SetActive(false);
        m_LooseContainer.SetActive(true);
        m_LevelCompletedButton.enabled = true;
        
        Button[] buttons = m_LooseContainer.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.enabled = true;
        }
    }

    private void ShowWinPopup()
    {
        m_LooseContainer.SetActive(false);
        m_WinContainer.SetActive(true);
        m_LevelCompletedButton.enabled = true;

        Button[] buttons = m_WinContainer.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            btn.enabled = true;
        }

        ParticleImage[] particleImages = m_WinContainer.GetComponentsInChildren<ParticleImage>();

        for (int i = 0; i < particleImages.Length; i++)
        {
            particleImages[i].Stop();
            particleImages[i].Play();
        }

    }

    private void HidePopup()
    {
        m_LooseContainer.SetActive(false);
        m_WinContainer.SetActive(false);
    }

    private void SetData(string levelCompletedText, Action levelCompletedButtonAction, string levelCompletedButtonText)
    {
        m_LevelCompletedText.text = levelCompletedText;

        m_LevelCompletedButton.onClick.RemoveAllListeners();
        m_LevelCompletedButton.enabled = true;

        // Add the listener correctly by passing a lambda that invokes the action
        m_LevelCompletedButton.onClick.AddListener(() => 
        {
            m_LevelCompletedButton.enabled = false;
            levelCompletedButtonAction?.Invoke();
        });

        m_LevelCompletedButtonText.text = levelCompletedButtonText; // Assuming you want to set the button text as well

    }

    private void ResetData()
    {
        m_LevelCompletedText.text = "";
        m_LevelCompletedButton.onClick.RemoveAllListeners();
        m_LevelCompletedButtonText.text = ""; // Assuming you want to set the button text as well
    }

    private void EnableRandomWinText()
    {
        for (int i = 0; i < m_WinText.Length; i++)
        {
            m_WinText[i].gameObject.SetActive(false);
        }
        int random = UnityEngine.Random.Range(0, m_WinText.Length);
        m_WinText[random].gameObject.SetActive(true);


    }

    public virtual void OnNextLevelButtonClicked()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            GameObject selectedObj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if (selectedObj != null)
            {
                Button btn = selectedObj.GetComponent<Button>();
                if (btn != null)
                {
                    btn.enabled = false;
                }
            }
        }
        
        m_LevelCompletedButton.enabled = false;
        //CloudUI.Instance.PlayColoudEnterAnimation();
    }


}
}