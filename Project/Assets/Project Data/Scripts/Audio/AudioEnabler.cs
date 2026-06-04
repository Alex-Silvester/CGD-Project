using System.Collections.Generic;
using UnityEngine;

public class AudioEnabler : MonoBehaviour
{
    [SerializeField] List<AudioSource> audio_source;
    [SerializeField] List<string> names;
    
    public void Enable(string name = "")
    {
        if (name == "") 
        {
            return; 
        }
        else if (!names.Contains(name)) 
        {
            Debug.LogWarning($"Audio Enabler for {gameObject.name} does not contain {name}"); 
            return; 
        }

        AudioSource current_source = audio_source[names.IndexOf(name)];

        current_source.Play();
    }

    public void Disable(string name = "")
    {
        if (name == "") return;

        else if (!names.Contains(name))
        {
            Debug.LogWarning($"Audio Enabler for {gameObject.name} does not contain {name}");
            return;
        }

        AudioSource current_source = audio_source[names.IndexOf(name)];

        if (!current_source.isPlaying) return;
        current_source.Stop();
    }
}
