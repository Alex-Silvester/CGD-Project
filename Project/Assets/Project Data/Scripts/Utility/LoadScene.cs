using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private string sceneToLoad;
	
    public void Load()
    {
		if (sceneToLoad == string.Empty)
		{
			Debug.LogWarning("Tried to load scene without a name.");
			return;
		}
		
        SceneManager.LoadScene(sceneToLoad);
    }
	
    public void Load(string sceneName)
    {
		if (sceneName == string.Empty)
		{
			Debug.LogWarning("Tried to load scene without a name.");
			return;
		}
		
        SceneManager.LoadScene(sceneName);
    }
	
	// Delay in seconds
    public void LoadWithDelay(float delay)
    {
        StartCoroutine(LoadWithDelayCoroutine(sceneToLoad, delay));
    }
	
	private IEnumerator LoadWithDelayCoroutine(string sceneToLoad, float delay)
	{
		yield return new WaitForSeconds(delay);
		
        SceneManager.LoadScene(sceneToLoad);
	}
}