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

    [SerializeField] Image starOne, starTwo, starThree;
    [SerializeField] Sprite starWhite, starYellow;

    private float timeCheck;

    private bool firstUpdate = true;

    public void ShowStars()                         //to change total required for max - change the maxScore in the ScoreObject object
    {
        if(firstUpdate)
        {
            firstUpdate = false;
            return;
        }

        ScoreUpdateUI.SetActive(true);
        float percentage =  (ScoreObject.CurrentScore / ScoreObject.maxScore);
        ScoreSlider.value = percentage;

        starOne.sprite = starWhite;
        starTwo.sprite = starWhite;
        starThree.sprite = starWhite;

        Debug.Log($"Current: {ScoreObject.CurrentScore}\nMax: {ScoreObject.maxScore}\nPercent: {percentage}");

        if(percentage >= 1f/3f)
        {
            starOne.sprite = starYellow;
        }

        if(percentage >= 2f/3f)
        {
            starTwo.sprite = starYellow;
        }

        if(percentage >= 1f)
        {
            starThree.sprite = starYellow;
        }

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
