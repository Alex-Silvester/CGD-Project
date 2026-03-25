using UnityEngine;
using UnityEngine.Events;

public class AutoLoadScene : LoadScene
{
	[Header("Settings")]
	[SerializeField] private float delay;
	[SerializeField] public UnityEvent OnStart;
	
	private void Start()
	{
		LoadWithDelay(delay);
		OnStart.Invoke();
	}
}