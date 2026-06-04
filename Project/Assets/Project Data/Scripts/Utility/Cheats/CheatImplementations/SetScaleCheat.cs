using UnityEngine;

public class SetScaleCheat : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Scale will only be changed if this cheat has been activated")]
	[SerializeField] private CheatCode cheatRequired;
	[SerializeField] private Transform transformToEffect;
	[SerializeField] private Vector3 newScale = new Vector3(2, 2, 2);
	
	private void Start()
	{
		if (!transformToEffect)
		{
			Debug.LogWarning("Transform to Effect not set in Set Scale Cheat script");
			return;	
		}
	
		
		if (cheatRequired.IsActivated())
		{
			transformToEffect.localScale = newScale;
		}
	}
}