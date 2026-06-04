using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	[SerializeField] private Vector3 rotation;
	
	private void Update()
	{
		transform.Rotate(rotation * Time.deltaTime);
	}
}