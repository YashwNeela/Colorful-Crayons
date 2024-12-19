using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetKits.ParticleImage;
using TMKOC.Sorting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedPopup : MonoBehaviour
{
    [SerializeField] private GameObject m_LooseContainer, m_WinContainer;
    [SerializeField] private TextMeshProUGUI m_LevelCompletedText;

    [SerializeField] private Button m_LevelCompletedButton;

    [SerializeField] private TextMeshProUGUI m_LevelCompletedButtonText;

    [SerializeField] private GameObject[] m_WinText;

    [SerializeField] private LevelFailUI m_LevelFailUI;


    void Awake()
    {
       
    }
    void OnEnable()
    {
        SortingGameManager.OnGameOver += OnGameOver;
        SortingGameManager.OnGameStart += OnGameStart;
        SortingGameManager.OnGameLoose += OnGameLoose;
        SortingGameManager.OnGameWin += OnGameWin;
        SortingGameManager.OnGameCompleted += OnGameCompleted;

    }

    private void OnGameOver()
    {
        Level currentLevel = LevelManager.Instance.GetCurrentLevel();
        if(currentLevel.Collectors == null){
            m_LevelFailUI.ToggleDetailContainer(true);
            return;
        }
       List<Collector> collectors = currentLevel.Collectors.ToList();
        List<SnapPoint> snapPoints = new List<SnapPoint>();

        for(int i =0;i<collectors.Count;i++)
        {
            if(!collectors[i].ShouldIncludeScore)
                continue;
            for(int j = 0;j<collectors[i].SnapPoints.Length;j++)
            {
                snapPoints.Add(collectors[i].SnapPoints[j]);
            }
        }
        m_LevelFailUI.ClearChildren();
        for(int i =0;i<snapPoints.Count;i++)
        {
            if(snapPoints[i].CurrentCollectible != null)
            m_LevelFailUI.SetLevelFailData(snapPoints[i].CurrentCollectible.spriteRenderer.sprite,
            snapPoints[i].CurrentCollectible.spriteRenderer.color,snapPoints[i].HasValidCollectible());
            else
            m_LevelFailUI.SetLevelFailData(null,Color.white, false,true);

            
        }
    }

    private void OnGameCompleted()
    {
        ResetData();
        HidePopup();
    }

    void OnDisable()
    {
        SortingGameManager.OnGameOver -= OnGameOver;

        SortingGameManager.OnGameStart -= OnGameStart;
        SortingGameManager.OnGameLoose -= OnGameLoose;
        SortingGameManager.OnGameWin -= OnGameWin;
        SortingGameManager.OnGameCompleted -= OnGameCompleted;


    }

    private void OnGameWin()
    {
        // SetData("Level Completed", Gamemanager.Instance.LoadNextLevel, "Next Level");
        EnableRandomWinText();
        ShowWinPopup();

    }

    private void OnGameLoose()
    {
        SetData("Oh no! Try Again!", SortingGameManager.Instance.GameRestart, "Restart");
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
    }

    private void ShowWinPopup()
    {
        m_LooseContainer.SetActive(false);
        m_WinContainer.SetActive(true);

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

    private void EnableRandomWinText()
    {
        for (int i = 0; i < m_WinText.Length; i++)
        {
            m_WinText[i].gameObject.SetActive(false);
        }
        int random = UnityEngine.Random.Range(0, m_WinText.Length);
        m_WinText[random].gameObject.SetActive(true);


    }

    public void OnNextLevelButtonClicked()
    {
        CloudUI.Instance.PlayColoudEnterAnimation();
    }


}
