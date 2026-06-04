using UnityEngine;

public class SetMeshCheat : MonoBehaviour
{
	[Header("Settings")]
	[Tooltip("Mesh will only be changed if this cheat has been activated")]
	[SerializeField] private CheatCode cheatRequired;
	[SerializeField] private SkinnedMeshRenderer meshToChange;
	[SerializeField] private Mesh newMesh;
	
	private void Start()
	{
		if (!meshToChange || !newMesh)
		{
			Debug.LogWarning("Mesh Filter or New Mesh not set in Set Mesh Cheat script");
			return;	
		}
	
		
		if (cheatRequired.IsActivated())
		{
			meshToChange.sharedMesh = newMesh;
		}
	}
}