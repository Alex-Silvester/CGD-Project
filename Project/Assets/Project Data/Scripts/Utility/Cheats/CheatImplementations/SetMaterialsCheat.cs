using UnityEngine;

public class SetMaterialsCheat : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Materials will only be changed if this cheat has been activated")]
	[SerializeField] private CheatCode cheatRequired;
	[SerializeField] private Renderer[] renderersToChange;
	[SerializeField] private Material newMaterial;
	
	private void Start()
	{
		if (renderersToChange.Length == 0)
		{
			Debug.LogWarning("Renderers to Change not set in Set Material Cheat script");
			return;	
		}
	
		
		if (cheatRequired.IsActivated())
		{
			// Change all renderers to specified material
			foreach (var rend in renderersToChange)
			{
				rend.material = newMaterial;
			}
		}
	}
}