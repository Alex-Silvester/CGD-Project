using UnityEngine;

[CreateAssetMenu(fileName = "LevelGroup", menuName = "ScriptableObjects/LevelGroup", order = 1)]
/// <summary>
/// Scriptable Object representing a group of levels
/// E.g. Warehouse with Warehouse 1-1, 1-2, etc as children
/// </summary>
public class LevelGroup : ScriptableObject
{
	[Tooltip("Name displayed to the player in the level select screen popup. E.g. Warehouse")]
	[SerializeField] private string displayName;
	[Tooltip("Reference in the Project Folder to the sprite that will be shown on the level select screen popup")]
	[SerializeField] private Sprite screenshot;
	
	#region Getters
	
	public string GetDisplayName()
	{
		return displayName;
	}
	
	public Sprite GetScreenshot()
	{
		return screenshot;
	}
	
	#endregion
}