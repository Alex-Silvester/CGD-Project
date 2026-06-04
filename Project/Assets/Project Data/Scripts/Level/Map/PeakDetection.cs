using UnityEngine;
using UnityEngine.Events;

public class PeakDetection : MonoBehaviour
{
	public static UnityEvent onCrateReached = new UnityEvent();
	
	private void OnTriggerEnter(Collider other)
	{		
		// Is it a crate?
		CrateObject crate = other.gameObject.GetComponent<CrateObject>();
		
		if (crate)
		{			
			// Let the achievement system know
			onCrateReached?.Invoke();
		}
	}
}