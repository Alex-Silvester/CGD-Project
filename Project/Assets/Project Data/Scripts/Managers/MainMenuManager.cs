using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public void LoadScene(string sceneToLoad)
	{
		if (sceneToLoad != string.Empty)
			SceneManager.LoadScene(sceneToLoad);
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}
}