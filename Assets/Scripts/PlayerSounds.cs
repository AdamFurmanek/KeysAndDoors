using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSounds : MonoBehaviour
{
    private Transform walkingSound, runningSound, scoreSound, hitSound;

    void Start()
    {
        walkingSound = transform.GetChild(0);
        runningSound = transform.GetChild(1);
        scoreSound = transform.GetChild(2);
        hitSound = transform.GetChild(3);
    }

    void Update()
    {
        if(GetComponent<NavMeshAgent>().velocity != new Vector3(0,0,0))
        {
            if (!GetComponent<PlayerController>().running)
            {
                Play(walkingSound);
                Stop(runningSound);
            }
            else
            {
                Stop(walkingSound);
                Play(runningSound);
            }

        }
        else
        {
            Stop(walkingSound);
            Stop(runningSound);
        }
    }

    private void Play(Transform sound)
    {
        if (!sound.GetComponent<AudioSource>().isPlaying)
        {
            sound.GetComponent<Song>().Play();
        }
    }

    private void Stop(Transform sound)
    {
        if (sound.GetComponent<AudioSource>().isPlaying)
        {
            sound.GetComponent<Song>().Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            hitSound.GetComponent<AudioSource>().Play();
        }
        if (other.gameObject.tag == "Key")
        {
            scoreSound.GetComponent<AudioSource>().Play();
        }
    }
}
