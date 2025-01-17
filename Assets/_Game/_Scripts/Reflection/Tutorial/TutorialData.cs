using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TMKOC{

public enum TutorialType
{
    World, Controls
}
[CreateAssetMenu(fileName = "TutorialStep", menuName = "ScriptableObject/Tutorial")]

public class TutorialData: SerializedScriptableObject
{
    public TutorialType type;

    public int tutorialId;

    public List<TutorialStep> tutorialSteps;
}
public class TutorialStep
{
    public TutorialType type;
    public string stepTitle;          // Title of the step
    [TextArea] public string description; // Description or instructions

   // [ShowIf("@type == TutorialStepType.World")]
    public Sprite imageSprite; // Object to highlight (optional)

   // [ShowIf("@type == TutorialStepType.Controls")]
    public GameObject controlObjects; // Object to highlight (optional)
    public Vector3 cameraFocusPosition; // Optional camera focus
    public float duration = 5f;       // Duration before auto-progressing (if applicable)

    public float delay = 0;

    public bool requiresEvent;        // Does the step wait for an event?
    public string eventName;          // Name of the event to wait for (optional)
}
public static class TutorialIds
{
    public static int movementTutorial = 1;

    public static int mirrorTutorial = 2;

    public static int objectsTutorial = 3;
}
}

