using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Cheats/Cheat Action")]
public class CheatAction : ScriptableObject
{
	public virtual void DoAction()
	{
		// Implement in child class (see CheatActionLoadScene.cs for example)
	}
}