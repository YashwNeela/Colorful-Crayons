using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TMKOC{

public enum TutorialType
{
    World, Controls
}
[CreateAssetMenu(fileName = "TutorialStep", menuName = "ScriptableObject/Tutorial")]

[System.Serializable]
public class TutorialData
{
    public TutorialType type;

    public int tutorialId;

    public List<TutorialStep> tutorialSteps;
}

[System.Serializable]
public class TutorialStep
{
    public TutorialType type;
    public string stepTitle;          // Title of the step
    [TextArea] public string description; // Description or instructions

   // [ShowIf("@type == TutorialStepType.World")]
    public Sprite imageSprite; // Object to highlight (optional)

   // [ShowIf("@type == TutorialStepType.Controls")]
    public GameObject controlObjects; // Object to highlight (optional)

    public List<string> enableControlsId;

    public List<string> disableControlsId;

    public Vector3 controlObjectPos;

    // [SerializeField]
    // public Dictionary<string,AudioClip> audioClip;

    public List<TutorialAudioClip> audioClip;
    //public AudioClip audioClip;

    public bool goBackToDefaultCamera = false;

    [Button]
    public void FectWorldPosOfControlObject()
    {
        controlObjectPos = controlObjects.transform.position;
    }
    public CinemachineCamera cameraToEnable; // Optional camera focus
    public float duration = 5f;       // Duration before auto-progressing (if applicable)

    public float delay = 0;

    public bool requiresEvent;        // Does the step wait for an event?
    public string eventName;          // Name of the event to wait for (optional)
}

[System.Serializable]
public struct TutorialAudioClip
{
    public AudioLanguage language;
    public AudioClip clip;
}
public static class TutorialIds
{
    public static int movementTutorial = 1;

    public static int jumpTutorial = 2;

    public static int mirrorTutorial = 3;

    public static int objectsTutorial = 4;
}
}

