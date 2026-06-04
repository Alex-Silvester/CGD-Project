using System.Collections;
using UnityEngine;

public class BreakAudio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Break()
    {
        transform.parent = null;
        GetComponent<AudioSource>().Play();

        StartCoroutine(destroyObj());
    }

    private IEnumerator destroyObj()
    {
        yield return new WaitUntil(notPlaying);

        Destroy(gameObject);
    }

    private bool notPlaying()
    {
        return !GetComponent<AudioSource>().isPlaying;
    }
}
