using UnityEngine;
using TMPro;

public class PlayerShowMessageController : MonoBehaviour
{
	[SerializeField] TMP_Text messageText;
	[SerializeField] TMP_Text interactText; // To make sure we aren't displaying text over top
	
	private void Update()
	{
		// Make sure we aren't displaying text over top
		// InteractionControl.cs can change text in Update so we need to match this
		if (interactText.text != string.Empty)
		{
			messageText.text = "";
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (interactText.text != string.Empty)
			return;
		
		// Is it a message area trigger?
		ShowMessageArea messageArea = other.GetComponent<ShowMessageArea>();
		
		if (messageArea)
		{
			// Show message text
			messageText.text = messageArea.GetMessage();
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		// Is it a message area trigger?
		ShowMessageArea messageArea = other.GetComponent<ShowMessageArea>();
		
		if (messageArea)
		{
			// Reset
			messageText.text = "";
		}
	}
}