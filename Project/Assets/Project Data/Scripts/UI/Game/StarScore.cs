using UnityEngine;
using UnityEngine.UI;
public class StarScore : MonoBehaviour
{
    [SerializeField]
    ScoreObject ScoreObject;

    [SerializeField]
    Slider ScoreSlider;

    [SerializeField]
    GameObject ScoreUpdateUI;

    private float timeCheck;

    public void ShowStars(bool _)                         //to change total required for max - change the maxScore in the ScoreObject object
    {
        ScoreUpdateUI.SetActive(true);
        float percentage =  (ScoreObject.CurrentScore / ScoreObject.maxScore);
        ScoreSlider.value = percentage;
        timeCheck = Time.time + 5;
    }

    // Update is called once per frame
    
    private void Update()
    {
        if (Time.time > timeCheck)
        {
            ScoreUpdateUI.SetActive(false);
        }
    }
}
