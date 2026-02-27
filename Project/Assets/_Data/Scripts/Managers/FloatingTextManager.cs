using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
	// Singleton
	public static FloatingTextManager instance { get; private set; }
	
	[Header("Prefab References")]
	[SerializeField] private FloatingText floatingTextPrefab;
	
	private void Awake()
	{
		// Singleton
		if (instance)
			Destroy(instance);
		
		instance = this;
	}
	
	public void Create(string newString, Vector3 position, Transform target, Color colour)
	{
		// Setup game object
		FloatingText floatingText = Instantiate(floatingTextPrefab, position, Quaternion.identity, target);
		
		// Setup TMP_Text
		floatingText.Setup(newString, colour, target);
	}
}