using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreObject", menuName = "Scriptable Objects/ScoreObject")]
public class ScoreObject : ScriptableObject
{   
    /// <summary>
    /// Maximum score can obtain in a stage.
    /// </summary>
    public float maxScore = 1000f;

    /// <summary>
    /// Fired when the score changes value.
    /// </summary>
    public Action<float> onScoreChanged;

    /// <summary>
    /// Current score held.
    /// </summary>
    public float CurrentScore { get; private set; }

    [SerializeField, Range(0f, 1f), Tooltip("Score ranges for each star to be rewarded, as a percentage.")]
    float starRangePercentage = 0.2f;

    /// <summary>
    /// Number of stars based on the current score.
    /// </summary>
    public int Stars => GetStars();

    /// <summary>
    /// Set the current score to a new value.
    /// </summary>
    public void SetScore(float value) => Set(value);

    /// <summary>
    /// Add a value to the current score.
    /// </summary>
    public void AddScore(float value) => Set(CurrentScore + value);

    private void OnEnable()
    {
        VictoryState.onExited += Clear;
    }

    private void OnDisable()
    {
        VictoryState.onExited -= Clear;
    }


    private void Set(float v)
    {
        CurrentScore = v;
        onScoreChanged?.Invoke(CurrentScore);
    }

    /* DISCLAIMER READ THIS ABOUT HOW SCORE RANGES WILL WORK:
    * (x / range) is only valid if star ranges are for each 20%.
    * If this were to change than you need to alter this value.
    * If we want to have variable size star ranges then this system would need to change.
    */

    // Returns how many stars there are based on the percentage of maximum score
    private int GetStars()
    {
        // Return the range index which x is in
        return Mathf.FloorToInt(Mathf.Clamp(CurrentScore / maxScore, 0, 1) / starRangePercentage);
    }

    /// <summary>
    /// Resets the score
    /// </summary>
    public void Clear() => Set(0);

}
