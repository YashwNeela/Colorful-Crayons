using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace TMKOC{

[System.Serializable]
public class TutorialDataSaver: SaveLoadBase
{
    [SerializeField] private List<int> m_tutorialCompletedDictKey;

    [SerializeField] private List<bool> m_tutorialCompletedDictBool;
    [SerializeField] private Dictionary<int, bool> m_tutorialCompletedDict;

    public Dictionary<int, bool> TutorialCompletedDict => m_tutorialCompletedDict;


     [SerializeField] float a;

    public void Save(Dictionary<int,bool> tutorialCompletedDict)
    {
        a = 1.1f;
        ConvertDictToKeyValuePair(tutorialCompletedDict);
        Serializer.SaveJsonData<TutorialDataSaver>(this,true);
    }

    private void ConvertDictToKeyValuePair(Dictionary<int,bool> tutorialCompletedDict)
    {
        m_tutorialCompletedDictKey = new List<int>();
        m_tutorialCompletedDictBool = new List<bool>();
        m_tutorialCompletedDict = new Dictionary<int, bool>();

        foreach(KeyValuePair<int, bool> keyValue in tutorialCompletedDict)
        {
            m_tutorialCompletedDictKey.Add(keyValue.Key);
            m_tutorialCompletedDictBool.Add(keyValue.Value);
        }

        m_tutorialCompletedDict = tutorialCompletedDict;
    }

    public Dictionary<int,bool> SetTutorialCompletedDict()
    { 
        if(m_tutorialCompletedDict != null)
            return m_tutorialCompletedDict;

        m_tutorialCompletedDict = new Dictionary<int, bool>();
        if(m_tutorialCompletedDictKey == null)
        {
            m_tutorialCompletedDictKey = new List<int>();
            m_tutorialCompletedDictKey.Add(TutorialIds.movementTutorial);
            m_tutorialCompletedDictKey.Add(TutorialIds.jumpTutorial);
            m_tutorialCompletedDictKey.Add(TutorialIds.mirrorTutorial);
            m_tutorialCompletedDictKey.Add(TutorialIds.objectsTutorial);


        }

        if(m_tutorialCompletedDictBool == null)
        {
            m_tutorialCompletedDictBool = new List<bool>();
            for(int i = 0;i< m_tutorialCompletedDictKey.Count;i++){
            m_tutorialCompletedDictBool.Add(false);
            }
        }

        for(int i = 0;i< m_tutorialCompletedDictKey.Count;i++)
        {
            m_tutorialCompletedDict.Add(m_tutorialCompletedDictKey[i],m_tutorialCompletedDictBool[i]);
        }

        return m_tutorialCompletedDict;

    }

    public TutorialDataSaver Load()
    {
        //ConverKeyValuePairToDict();
        return Serializer.LoadJsonData<TutorialDataSaver>(this);
    }

    public void Clear()
    {
        Serializer.DeleteFile<TutorialDataSaver>(this);
    }

    public void OpenFolder()
    {
        Serializer.OpenFolder<TutorialDataSaver>(this);
    }

}



public class TutorialManager : SerializedSingleton<TutorialManager>
{
    public Action<int> OnTutorialStarted;

    public Action<int> OnTutorialEnded;
    public TutorialDataSaver m_TutorialDataSaver;

    public bool IsTutorialCompleted(int tutorialId)
    {
        try{
            return m_TutorialDataSaver.TutorialCompletedDict[tutorialId];
        }
        catch
        {
            Debug.LogError("No tutorial with id: " + tutorialId + " found");
            return false;;
        }
    }

    public List<TutorialData> tutorialData; // Steps in the tutorial

    public TutorialData currentTutorialData;

    public TutorialStep GetCurrentTutorialStep()
    {
        return currentTutorialData.tutorialSteps[currentStepIndex];
    }
    private int currentStepIndex = 0;

    public TutorialUI tutorialUI;  // Reference to the UI manager
    public Camera mainCamera;      // Main camera for focusing objects

    public bool m_IsTutorialActive;

    public bool IsTutorialActive => m_IsTutorialActive;

    private CinemachineCamera m_DefaultCinemachineCamera;
    


    private void Start()
    {
        m_DefaultCinemachineCamera = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineCamera;
        FetchTutorialData();
       // StartTutorial(TutorialIds.movementTutorial);
    }

    void FetchTutorialData()
    {
        TutorialDataSaver TutorialDataSaverTemp = m_TutorialDataSaver.Load();
        if(TutorialDataSaverTemp == null){
            // Serializer.SaveJsonData<TutorialDataSaver>(m_TutorialDataSaver,true);
            Dictionary<int,bool> tempDict = new Dictionary<int, bool>();
            tempDict.Add(TutorialIds.movementTutorial,false);
            tempDict.Add(TutorialIds.jumpTutorial,false);

            tempDict.Add(TutorialIds.mirrorTutorial,false);

            tempDict.Add(TutorialIds.objectsTutorial,false);

            m_TutorialDataSaver.Save(tempDict);
            return;
            
        }
        m_TutorialDataSaver = TutorialDataSaverTemp;
    }

    [Button]
    public void DeleteTutorialData()
    {
        m_TutorialDataSaver.Clear();
    }

    [Button]
    public void OpenDataFolder()
    {
        m_TutorialDataSaver.OpenFolder();
    }
    public void StartTutorial(int tutorialId)
    {
        currentTutorialData = tutorialData.Find(data => data.tutorialId == tutorialId);
        if(m_TutorialDataSaver.SetTutorialCompletedDict()[currentTutorialData.tutorialId] == true)
        {
            Debug.Log("Tutorial Already completed");
            return;
        }

        if (currentTutorialData.tutorialSteps.Count == 0)
        {
            Debug.LogWarning("No tutorial steps found!");
            m_IsTutorialActive = false;
            return;
        }
        m_IsTutorialActive = true;

        ShowStep(currentStepIndex);
        OnTutorialStarted?.Invoke(tutorialId);
    }

    private void ShowStep(int index)
    {
        
        if (index >= currentTutorialData.tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }
        
        var step = currentTutorialData.tutorialSteps[index];
       

        if (step.imageSprite != null)
        {
            //HighlightObject(step.highlightObject);
        }

        if (step.cameraToEnable != null)
        {
            FocusCamera(step.cameraToEnable);
           StartCoroutine(StaticCoroutine.Co_GenericCoroutine(1,()=>
           {
            StartCoroutine(StaticCoroutine.Co_WaitUntil(()=> Camera.main.GetComponent<CinemachineBrain>().IsBlending
            ,()=>
            {
            tutorialUI.ShowStep(step);

            }));  
           }));
        }
        else
        {
            tutorialUI.ShowStep(step);
            
        }
        if (step.requiresEvent)
        {
            TutorialEventManager.Instance.Subscribe(step.eventName, OnStepEventTriggered);
        }
        else
        {
            StartCoroutine(AutoAdvanceStep(step.duration));
        }
    }

    private IEnumerator AutoAdvanceStep(float duration)
    {
        yield return new WaitForSeconds(duration);
        NextStep();
    }

    private void OnStepEventTriggered()
    {
        TutorialEventManager.Instance.Unsubscribe(currentTutorialData.tutorialSteps[currentStepIndex].eventName, OnStepEventTriggered);
        NextStep();
    }

    private void NextStep()
    {
        currentStepIndex++;
        tutorialUI.Hide();
        if(currentStepIndex >= currentTutorialData.tutorialSteps.Count){
            EndTutorial();
            return;
        }
        StartCoroutine(StaticCoroutine.Co_GenericCoroutine(currentTutorialData.tutorialSteps[currentStepIndex].delay,()=>
        {
            ShowStep(currentStepIndex);
        }));

    }

    private void HighlightObject(GameObject obj)
    {
        // Implement object highlighting logic here (e.g., using a shader or outline effect)
    }

    private void FocusCamera(CinemachineCamera cinemachineCamera)
    {
        m_DefaultCinemachineCamera.Priority = -1;
        cinemachineCamera.Priority = 1;
    }

    private void EndTutorial()
    {
        OnTutorialEnded?.Invoke(currentTutorialData.tutorialId);
        m_TutorialDataSaver.SetTutorialCompletedDict()[currentTutorialData.tutorialId] = true;
        m_TutorialDataSaver.Save(m_TutorialDataSaver.SetTutorialCompletedDict());
        currentTutorialData = null;
        currentStepIndex = 0;
        Debug.Log("Tutorial Complete!");
        tutorialUI.Hide();
        m_IsTutorialActive = false;
        (Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineCamera).Priority = -1;
        m_DefaultCinemachineCamera.Priority = 1;

    }


}
}