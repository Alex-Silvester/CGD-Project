using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
	// Used for back button click
	public void SaveSettings()
	{
		if (SaveManager.instance)
			SaveManager.instance.Save();
		else
			Debug.LogWarning("Save Manager not found");
	}
}