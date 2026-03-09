using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles displaying a time on a clock.
/// Currently functions by updating the clock each <see cref="CollectorScheduler.OnRunningUpdate"/> call to display the
/// remaining time for its <see cref="CollectorScheduler.RemainingQuotaTime"/>.
/// </summary>
public class SceneQuotaClock : MonoBehaviour
{
    [SerializeField]
    TextMeshPro clockText;

    [SerializeField]
    CollectorScheduler scheduler;

    // Is in the format MM:SS.
    string GetTimeString(float time) => $"{TimeSpan.FromSeconds(time):mm\\:ss}";

    // Update clock with the new time. Fired every scheduler.Update call.
    void OnRunningUpdate(float time) => clockText.SetText(GetTimeString(time));


    private void OnEnable()
    {
        if (scheduler == null) return;

        scheduler.OnRunningUpdate += OnRunningUpdate;
    }

    private void OnDisable()
    {
        if (scheduler == null) return;

        scheduler.OnRunningUpdate -= OnRunningUpdate;
    }


}
