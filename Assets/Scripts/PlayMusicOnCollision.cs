using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayMusicOnCollision : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("triggered");
        if (other.CompareTag("Player"))
        {
		Debug.Log("collision check");

           if (!audioSource.isPlaying)
            {
		Debug.Log("condition check");
                audioSource.Play();
            }
		else 
		{
		 audioSource.Pause();
		}
            
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("triggered");

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("collision check");

            if (!audioSource.isPlaying)
            {
                Debug.Log("condition check");
                audioSource.Play();
            }
            else
            {
                audioSource.Pause();
            }

        }
    }
}