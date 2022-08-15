using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{

    public AudioSource src;
    public AudioClip walkLeftFoot;
    public AudioClip walkRightFoot;
    public AudioClip jumpUp;
    // public AudioClip jumpLand;
    public AudioClip swing;
    public AudioClip damaged;

    void WalkLeftFoot()
    {
        src.clip = walkLeftFoot;
        src.Play();
    }

    void WalkRightFoot()
    {
        src.clip = walkRightFoot;
        src.Play();
    }

    void JumpUp()
    {
        src.clip = jumpUp;
        src.Play();
    }

    /*
    void JumpLand()
    {
        src.clip = jumpLand;
        src.Play();
    }
    */

    void Swing()
    {
        src.clip = swing;
        src.Play();
    }

    void Damaged()
    {
        src.clip = damaged;
        src.Play();
    }

}
