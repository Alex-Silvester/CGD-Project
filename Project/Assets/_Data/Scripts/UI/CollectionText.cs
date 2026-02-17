using UnityEngine;
using TMPro;
using System.Collections;

public class CollectionText : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, Range(0f, 10f)]
    float visbilityDuration = 3f;

    WaitForSeconds interval;

    TimeManager TM => TimeManager.instance;

    private void Awake()
    {
        interval = new WaitForSeconds(visbilityDuration);
        gameObject.SetActive(false);
    }

    // This can be assigned to an event to update the displayed text
    public void UpdateText(bool passed)
    {
        // The "Quota updated" text will only appear if the current game state is in the playing state
        display.SetText($"{(passed ? "Passed" : "Missed")} quota\r\n{(TM.CurrentScheduleObject.actingScheduler.Schedule.Count > 0 ? "Quota updated" : "")}");
        gameObject.SetActive(true);
        StartCoroutine(TogglePanelVisibility());
    }

    // Toggles displaying the text after a delay
    IEnumerator TogglePanelVisibility()
    {
        yield return interval;
        gameObject.SetActive(false);
    }
}