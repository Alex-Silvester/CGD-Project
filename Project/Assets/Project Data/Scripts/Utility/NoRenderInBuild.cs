using UnityEngine;

public class NoRenderInBuild : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
#if !UNITY_EDITOR
        GetComponent<Renderer>().enabled = false;
#endif
    }
}
