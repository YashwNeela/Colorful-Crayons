using Sirenix.OdinInspector;
using UnityEngine;

namespace TMKOC{

public enum TutorialStepType
{
    World, Controls
}
[CreateAssetMenu(fileName = "TutorialStep", menuName = "ScriptableObject/Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    public TutorialStepType type;
    public string stepTitle;          // Title of the step
    [TextArea] public string description; // Description or instructions

    [ShowIf("@type == TutorialStepType.World")]
    public Sprite imageSprite; // Object to highlight (optional)

    [ShowIf("@type == TutorialStepType.Controls")]
    public GameObject controlObjects; // Object to highlight (optional)
    public Vector3 cameraFocusPosition; // Optional camera focus
    public float duration = 5f;       // Duration before auto-progressing (if applicable)

    public float delay = 0;

    public bool requiresEvent;        // Does the step wait for an event?
    public string eventName;          // Name of the event to wait for (optional)
}
}