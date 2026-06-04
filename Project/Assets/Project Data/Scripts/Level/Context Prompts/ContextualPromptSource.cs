using UnityEngine;

/// <summary>
/// Source object for displaying a contextual prompt. A ContextualPromptListener will detect this
/// </summary>
public class ContextualPromptSource : MonoBehaviour
{
    // SIMPLE IMPLEMENTATION i want this to be expanded on later
    // Because of that some of the things here are gonna go unused for a bit

    [SerializeField, Tooltip("If the prompt should appear with ray/box casts.")]
    bool showOnCast = false;

    [SerializeField, Tooltip("If the prompt should appear with trigger collisions")]
    bool showOnTrigger = false;

    [TextArea]
    public string message = "";

    public bool ShowOnCast => showOnCast;
    public bool ShowOnTrigger => showOnTrigger;
}
