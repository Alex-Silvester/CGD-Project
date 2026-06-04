using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerGatherPoint : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private int requiredPlayerCount = 4;
	[SerializeField] private UnityEvent onPlayerCountReached = new UnityEvent();
	
	[Header("Cache")]
	[SerializeField] private TMP_Text[] playerCountTexts;
	
	private int playerCount;
	private bool active = true;
	
    private void OnTriggerEnter(Collider other)
    {
		// Prevent triggering more than once
		if (!active)
			return;
		
        // Is it a player?
		if (other.CompareTag("Player"))
		{
			playerCount++;
			
			UpdateText();
			
			// Has the player count been reached?
			if (playerCount >= requiredPlayerCount)
			{
				onPlayerCountReached?.Invoke();
				
				// Prevent being triggered more than once
				active = false;
			}
		}
    }
	
    private void OnTriggerExit(Collider other)
    {
        // Is it a player?
		if (other.CompareTag("Player"))
		{
			playerCount--;
			
			UpdateText();
		}
    }
	
	private void UpdateText()
	{
		if (playerCountTexts.Length > 0)
		{
			foreach (var playerCountText in playerCountTexts)
			{
				playerCountText.text = playerCount + " / " + requiredPlayerCount;
			}
		}
	}
}