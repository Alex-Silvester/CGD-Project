using TMPro;
using UnityEngine;

public class TotalScoreText : MonoBehaviour
{
    [SerializeField]
    ScoreObject scoreObject;

    [SerializeField]
    TextMeshProUGUI display;

    [SerializeField, TextArea]
    string displayText = "SCORE: ";

    private void OnEnable()
    {
        scoreObject.onScoreChanged += UpdateText;
    }

    private void OnDisable()
    {
        scoreObject.onScoreChanged -= UpdateText;
    }

    // This can be assigned to an event to update the displayed text
    public void UpdateText(float score)
    {
        display.SetText(displayText + score);
    }
}
