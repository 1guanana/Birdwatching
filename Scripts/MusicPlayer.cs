using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    static MusicPlayer instance = null;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else {
            instance = this;
            DontDestroyOnLoad(transform.gameObject);
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayMusic()
    {
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
