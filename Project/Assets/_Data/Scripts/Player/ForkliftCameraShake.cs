using System.Collections;
using UnityEngine;

/// <summary>
/// Source - https://www.youtube.com/watch?v=9A9yj8KnM8c (Brackeys)
/// Component to shake the player's camera
/// Subscribes to events and shakes when necessary
/// </summary>
public class ForkliftCameraShake : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float quotaMissedDuration = 0.5f;
	[SerializeField] private float quotaMissedMagnitude = 0.1f;
	
	[Header("Cache")]
	[Tooltip("Reference to the player camera holder (root) transform")]
	[SerializeField] private Transform cameraHolder;
	
	//Event objects
	private CrateCollector crateCollector;
	
	// Code needs to run after DrivingController Awake()
	private void Start()
	{
		// Reset after being detached by DrivingController.cs
		transform.localPosition = Vector3.zero;
		transform.rotation = Quaternion.identity;
		
		// Get event objects
		
		// Forklift spawned dynamically so can't use inspector
		crateCollector = GameObject.Find("CrateCollector").GetComponent<CrateCollector>();
		
		// Subscribe to events
		if (crateCollector)
		{
			crateCollector.onEvaluatedRequirement.AddListener(OnQuotaEvaluation);
		}
		else
		{
			Debug.LogWarning("Forklift Camera Shake couldn't find Crate Collector");
		}
	}
	
	// Cause the camera to shake by specified amount
	public void Shake(float duration, float magnitude)
	{
		StartCoroutine(ShakeCoroutine(duration, magnitude));
	}
	
	// Make camera shaking happen over time (rather than for one frame)
	private IEnumerator ShakeCoroutine(float duration, float magnitude)
	{
		// Make sure camera is reset to its starting position
		Vector3 originalPosition = cameraHolder.localPosition;
		
		float elapsed = 0;
		
		while (elapsed < duration)
		{
			// Offset on X and Y by a random amount each frame
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;
			
			// No shaking on the Z axis
			cameraHolder.localPosition = new Vector3(x, y, originalPosition.z);
			
			elapsed += Time.deltaTime;
			
			// Wait for next frame
			yield return null;
		}
		
		// Snap back to starting position
		cameraHolder.localPosition = originalPosition;
	}
	
	#region Events
	
	private void OnQuotaEvaluation(bool isSuccess)
	{
		// Shake if quota failed
		if (!isSuccess)
		{
			Shake(quotaMissedDuration, quotaMissedMagnitude);
		}
	}
	
	#endregion Events
}