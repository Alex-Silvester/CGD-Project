using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Listener for contextual prompts, displaying their message to the relevant target.
/// </summary>
public class ContextualPromptListener : MonoBehaviour
{
    /* NOTE THAT THE CURRENT DISPLAY FOR THIS IS TEMPORARY:
     * The display should be handled by the ContextualPromptSource, not the the listener.
     * Right now the listener will display the text using the old system (Similar to PlayerShowMessageController)
     */

    [SerializeField]
    Collider areaCollider;

    [SerializeField]
    TextMeshProUGUI messageUI;

    [SerializeField]
    float boxCastSize = 1f;

    [SerializeField]
    float boxCastDistance = 3f;

    RaycastHit hit;
    ContextualPromptSource currentFocus, castSource, triggerSource;

    private void OnValidate()
    {
        if (areaCollider == null)
        {
            TryGetComponent(out areaCollider);
        }
    }

    private void Awake()
    {
        currentFocus = triggerSource = null;
    }

    private void FixedUpdate()
    {
        CastBox();
        if (currentFocus != null)
        {
            DisplayPrompt(currentFocus);
        }
        else
        {
            HidePrompt();
        }
    }

    /* Source detection behaviour:
    BoxCast to find sources in front, collider to find sources nearby.
    Behaviour:
        - For that source detection find a source
        - If there is a source, if we do not have a focus assign that new source as the focus
        - If there is no source, declare this source detection has returned null, if the other source detection was null then clear the focus
     */

    // Does a BoxCast to see if there's any sources to trigger
    private void CastBox()
    {
        var success = Physics.BoxCast(transform.position, 0.5f * boxCastSize * transform.localScale, transform.forward, out hit, transform.rotation, boxCastDistance);
        if (success)
        {
            // Get the prompt source and assign to focus if we don't have one
            if (TryFindSourceInObject(hit.collider.gameObject, out var source))
            {
                //Debug.Log("Hit a prompt source");
                if (source.ShowOnCast)
                {
                    castSource = source;
                    if (currentFocus == null) currentFocus = castSource;
                }
            }
        }
        else
        {
            // Mark no source found; clear focus if the other check is empty.
            castSource = null;
            if (triggerSource == null) currentFocus = null;
        }
    }

    // Only assign a new trigger source if we do not already have one
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Float" || other.gameObject.tag == "Player")
            messageUI.gameObject.SetActive(true);

        if (triggerSource != null) return;

        // Check for this and its children
        if(TryFindSourceInObject(other.gameObject, out var source))
        {
            if (source.ShowOnTrigger)
            {
                triggerSource = source;
                if (currentFocus == null) currentFocus = triggerSource;
            }
        }
    }

    // When exiting a trigger, always mark the trigger source as null
    private void OnTriggerExit(Collider other)
    {
        messageUI.gameObject.SetActive(false);

        if (TryFindSourceInObject(other.gameObject, out _))
        {
            triggerSource = null;
            if (castSource == null) currentFocus = null;
        }
    }

    // Checks if the source is on the object or a child of the object
    bool TryFindSourceInObject(GameObject gameObject, out ContextualPromptSource source)
    {
        if (!gameObject.TryGetComponent(out source))
        {
            source = gameObject.GetComponentInChildren<ContextualPromptSource>();
            if (source == null) return false;
        }

        return true;
    }

    // Display's the message of the given prompt source
    void DisplayPrompt(ContextualPromptSource source)
    {
        messageUI.SetText(source.message);
        messageUI.gameObject.SetActive(true);
    }

    void HidePrompt()
    {
        messageUI.SetText("");
        messageUI.gameObject.SetActive(false);
    }

}
