using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AudioSource src;
    public AudioClip walk;
    public AudioClip attack;
    public AudioClip overloaded;
    public AudioClip drained;

    void Walk()
    {
        src.clip = walk;
        src.Play();
    }

    void Attack()
    {
        src.clip = attack;
        src.Play();
    }

    void OverLoaded()
    {
        src.clip = overloaded;
        src.Play();
    }
}
