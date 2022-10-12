using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a desired clip when an action is performed by the player
/// </summary>
public class PlayerSFX : MonoBehaviour
{

    public AudioSource src;
    public AudioClip walkLeftFoot;
    public AudioClip walkRightFoot;
    public AudioClip jumpUp;
    // public AudioClip jumpLand;
    public AudioClip swing;
    public AudioClip damaged;


    /**
        * Plays the audio clip for walking on the left foot
    **/
    void WalkLeftFoot()
    {
        src.clip = walkLeftFoot;
        src.Play();
    }

    /**
        * Plays the audio clip for walking on the right foot
    **/
    void WalkRightFoot()
    {
        src.clip = walkRightFoot;
        src.Play();
    }

    /**
        * Plays the audio clip for jumping
    **/
    void JumpUp()
    {
        src.clip = jumpUp;
        src.Play();
    }

    /*
    /**
        * Plays the audio clip for landing
    **/
    void JumpLand()
    {
        src.clip = jumpLand;
        src.Play();
    }
    */

    /**
        * Plays the audio clip for swinging
    **/
    void Swing()
    {
        src.clip = swing;
        src.Play();
    }

    /**
        * Plays the audio clip for getting damaged
    **/
    void Damaged()
    {
        src.clip = damaged;
        src.Play();
    }

}
