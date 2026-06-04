using UnityEngine;
using UnityEngine.Events;

public class SecretAreaTrigger : MonoBehaviour
{
	// Events
	public static UnityEvent onTriggered = new UnityEvent();
	
	private void OnTriggerEnter(Collider other)
	{
		// Is it a forklift?
		if (other.CompareTag("Player"))
		{
			onTriggered?.Invoke();
			
			// Prevent re-triggering
			Destroy(this);
		}
	}
}