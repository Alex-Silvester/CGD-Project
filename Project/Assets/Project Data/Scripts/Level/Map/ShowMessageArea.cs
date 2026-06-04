using UnityEngine;

public class ShowMessageArea : MonoBehaviour
{
	[SerializeField] [TextArea] private string message;
	
	#region Getters
	
	public string GetMessage()
	{
		return message;
	}
	
	#endregion Getters
}