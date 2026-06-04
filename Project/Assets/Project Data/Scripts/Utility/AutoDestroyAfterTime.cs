using System.Collections;
using UnityEngine;

public class AutoDestroyAfterTime : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float delay = 1.0f;
	
	private void Start()
	{
		StartCoroutine(DestroyCoroutine());
	}
	
	private IEnumerator DestroyCoroutine()
	{
		yield return new WaitForSeconds(delay);
		
		Destroy(gameObject);
	}
}