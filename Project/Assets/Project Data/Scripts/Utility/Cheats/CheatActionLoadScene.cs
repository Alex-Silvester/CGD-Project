using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Scriptable Objects/Cheats/Cheat Action Load Scene")]
public class CheatActionLoadScene : CheatAction
{
	[Header("Settings")]
	[SerializeField] private string sceneToLoad;
	
	public override void DoAction()
	{
		SceneManager.LoadScene(sceneToLoad);
	}
}