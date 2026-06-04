using System.Collections;
using UnityEngine;
using TMPro;

public class RandomText : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private string[] potentialStrings;
	[Tooltip("Optional. Used for manual number")]
	[SerializeField] private string arrayIndexString;
	
	[SerializeField] private bool shouldRefresh = false;
	[SerializeField] private float minimumRefreshDelay = 3f;
	[SerializeField] private float maximumRefreshDelay = 5f;
	
	[Header("Cache")]
	[SerializeField] TMP_Text text;
	[Tooltip("Optional. Used for manual number")]
	[SerializeField] TMP_Text arrayIndexText;
	
    void Start()
    {
        RandomiseText();
		
		if (shouldRefresh)
		{
			StartCoroutine(RandomiseTextCoroutine());
		}
    }
	
	private void RandomiseText()
	{		
		int rand = Random.Range(0, potentialStrings.Length);
		
		text.text = potentialStrings[rand];
		
		// Show hint index number
		if (arrayIndexText)
		{
			// +1 to offset 0 based array index
			arrayIndexText.text = arrayIndexString + (rand + 1);
		}
	}
	
	private IEnumerator RandomiseTextCoroutine()
	{
		// Randomise text until loading screen is exited
		while(true)
		{
			yield return new WaitForSeconds(Random.Range(minimumRefreshDelay, maximumRefreshDelay));
			
			RandomiseText();
		}
	}
}