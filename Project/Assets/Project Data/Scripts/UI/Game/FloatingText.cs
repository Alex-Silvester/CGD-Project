using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private TMP_Text text;
	
	// Rotate to face this transform (billboard)
	private Transform faceTarget;
	
	public void Setup(string newString, Color colour, Transform target)
	{
		text.text = newString;
		text.color = colour;
		faceTarget = target;
	}
	
	private void Update()
	{
		// Look at the target, but not on the Y axis
		// Needs to be inverted otherwise the text is backwards
		if (faceTarget)
		{
			// Ignore Y axis
			Vector3 targetPosition = faceTarget.position;
			targetPosition.y = transform.position.y;
			
			// Source - https://discussions.unity.com/t/lookat-in-opposite-direction/23429
			transform.LookAt(2 * transform.position - targetPosition);
		}
	}
}