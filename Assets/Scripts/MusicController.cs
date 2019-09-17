using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioSource[] music;

    private void Awake()
    {
        music = GetComponents<AudioSource>();
        Debug.Log(music.Length);
    }

    private void Start()
    {
        PlayNextSong();
    }

    void PlayNextSong()
    {
        AudioSource track = music[Random.Range(0, music.Length)];
        track.Play();
        Debug.Log("Playing track " + track.clip.name);
        Invoke("PlayNextSong", track.clip.length);
    }
}
