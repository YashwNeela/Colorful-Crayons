using System;
using UnityEngine;


namespace TMKOC.StarLink{

public enum TutorialPlayMode
{
    PlayOnce,
    PlayEveryTime,
    PlayUntilCompleted
}
[System.Serializable]
public class TutorialData
{
    public string tutorialId;
    public TutorialPlayMode playMode;
    public TutorialStep[] steps;

    public Action OnTutorialComplete;
}


[System.Serializable]
public class TutorialStep
{
    public TutorialStepUI stepUI;
    [TextArea(2, 5)]
    public string message;

    public bool clickAnywhere;

    public GameObject targetObject;

    public AudioClip audioClip;

    

    public bool waitForClickOnTarget = true;

    public float autoNextDelay = 0f;
}



}