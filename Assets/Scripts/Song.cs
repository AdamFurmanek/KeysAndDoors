using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Song : MonoBehaviour
{
    private AudioSource audioSource;
    public int howLong;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void Stop()
    {
        if (audioSource.isPlaying)
        {
            StartCoroutine(Stopping());
        }
    }

    IEnumerator Stopping()
    {
        for(int i = 0; i < howLong; i++)
        {
            yield return new WaitForSeconds(0.1f);
            audioSource.volume -= 1.0f/howLong;
        }
        audioSource.Stop();
        audioSource.volume = 1;
    }

}
