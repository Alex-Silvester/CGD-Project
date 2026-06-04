using UnityEngine;

public class ForkliftLights : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private CratePickUp forklift;
	[SerializeField] private GameObject[] pickupLights;
	
    private void Awake()
    {
        // Subscribe to events
		forklift.onGrabbed.AddListener(OnGrabbed);
		forklift.onDropped.AddListener(OnDropped);
    }
	
	private void OnGrabbed()
	{
		// Enable light
		foreach (var light in pickupLights)
		{
			light.SetActive(true);
		}
	}
	
	private void OnDropped()
	{
		// Disable lights
		foreach (var light in pickupLights)
		{
			light.SetActive(false);
		}
	}
}