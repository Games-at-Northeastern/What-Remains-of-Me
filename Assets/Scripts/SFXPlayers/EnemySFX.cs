using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Plays a desired clip when an action is performed by the enemy
/// </summary>
public class EnemySFX : MonoBehaviour
{
    public AudioSource src;
    public AudioClip walk;
    public AudioClip attack;
    public AudioClip overloaded;
    public AudioClip drained;

    /**
        * Plays the audio clip for walking
    **/
    void Walk()
    {
        src.clip = walk;
        src.Play();
    }

    /**
        * Plays the audio clip for attacking
    **/
    void Attack()
    {
        src.clip = attack;
        src.Play();
    }

    /**
        * Plays the audio clip for being overloaded
    **/
    void OverLoaded()
    {
        src.clip = overloaded;
        src.Play();
    }
}
