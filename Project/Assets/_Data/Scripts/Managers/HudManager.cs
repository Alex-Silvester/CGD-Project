using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudManager : MonoBehaviour
{
	[Header("Enter Vehicle")]
	[SerializeField] private GameObject[] enterVehiclePrompts; // 0 = unused. 1 = player 1 etc.
	[SerializeField] private Image[] enterVehicleProgressSliders; // 0 = unused. 1 = player 1 etc.
	[SerializeField] private TMP_Text[] enterVehiclePromptTexts; // 0 = unused. 1 = player 1 etc.
	
	[Header("Quota Requirements")]
	[SerializeField] private RectTransform quotaRequirementRectTransform;
	[SerializeField] private Vector2 quotaRequirementThreePlayerAnchor;
	[SerializeField] private Vector2 quotaRequirementThreePlayerOffset;
	
	[Header("Total Score")]
	[SerializeField] private RectTransform totalScoreRectTransform;
	[SerializeField] private Vector2 totalScoreAnchor;
	[SerializeField] private Vector2 totalScoreOffset;
	
	[Header("Total Time")]
	[SerializeField] private RectTransform totalTimeRectTransform;
	[SerializeField] private Vector2 totalTimeAnchor;
	[SerializeField] private Vector2 totalTimeOffset;
	
	private void Start()
	{
		// Is the scren split into four sections?
		//if (LobbyMenuManager.currentPlayers.Count > 2)
		//{
			// Align UI along central column
			SetupFourQuadrantUI();
		//}
	}
	
	public void SetVehiclePromptStatus(int playerNumber, bool newStatus)
	{
		if (playerNumber <= 0)
			return;
		
		enterVehiclePrompts[playerNumber].SetActive(newStatus);
	}
	
	public void SetVehiclePromptProgress(int playerNumber/*, float amount*/)
	{
		if (playerNumber <= 0)
			return;
		
		//enterVehicleProgressSliders[playerNumber].fillAmount = amount;
	}
	
	public void SetVehiclePromptText(int playerNumber, string newText)
	{
		if (playerNumber <= 0)
			return;
		
		enterVehiclePromptTexts[playerNumber].text = newText;
	}
	
	private void SetupFourQuadrantUI()
	{
		// Setup quota requirements UI
		quotaRequirementRectTransform.anchorMin = new Vector2(quotaRequirementThreePlayerAnchor.x, quotaRequirementThreePlayerAnchor.y);
        quotaRequirementRectTransform.anchorMax = new Vector2(quotaRequirementThreePlayerAnchor.x, quotaRequirementThreePlayerAnchor.y);
		quotaRequirementRectTransform.anchoredPosition = new Vector2(quotaRequirementThreePlayerOffset.x, quotaRequirementThreePlayerOffset.y); // Reset
		
		// Total score UI
		totalScoreRectTransform.anchorMin = new Vector2(totalScoreAnchor.x, totalScoreAnchor.y);
        totalScoreRectTransform.anchorMax = new Vector2(totalScoreAnchor.x, totalScoreAnchor.y);
		totalScoreRectTransform.anchoredPosition = new Vector2(totalScoreOffset.x, totalScoreOffset.y); // Reset
		
		// Total score UI
		totalTimeRectTransform.anchorMin = new Vector2(totalTimeAnchor.x, totalTimeAnchor.y);
        totalTimeRectTransform.anchorMax = new Vector2(totalTimeAnchor.x, totalTimeAnchor.y);
		totalTimeRectTransform.anchoredPosition = new Vector2(totalTimeOffset.x, totalTimeOffset.y); // Reset
	}
}