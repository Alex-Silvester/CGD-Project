using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;

public class LoadingVariables : MonoBehaviour
{
    public string sceneName = "";

    AsyncOperation load;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);

        StartCoroutine(preLoadLoadingScene());
    }

    private IEnumerator preLoadLoadingScene()
    {
        load = SceneManager.LoadSceneAsync("Loading Scene");
        load.allowSceneActivation = false;

        yield return load;
        yield return null;
    }

    public void activateScene()
    {
        load.allowSceneActivation = true;
    }

    public void allowVariableDestruction()
    { 
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
    }
}
