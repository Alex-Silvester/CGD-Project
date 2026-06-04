using UnityEngine;

public class DeleteSaveData : MonoBehaviour
{
	public void Delete()
	{
		if (SaveManager.instance)
			SaveManager.instance.ClearSave();
	}
}