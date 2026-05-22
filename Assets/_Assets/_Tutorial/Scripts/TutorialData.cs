using System;
using UnityEngine;


public enum TutorialPlayMode
{
    PlayOnce,
    PlayEveryTime,
    PlayUntilCompleted
}

[CreateAssetMenu(menuName = "Tutorial/Tutorial Data")]
public class TutorialData : ScriptableObject
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


