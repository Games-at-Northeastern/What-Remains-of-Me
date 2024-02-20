using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Plays the designated audio clip for the player's actions
/// <summary>
public class PlayerSFX : MonoBehaviour
{

    /// <summary>
    /// Plays the left foot's walking audio clip at the player's location
    /// <summary>
    public void WalkLeftFoot()
    {
        SoundController.instance.PlaySound("Player_WalkLeftFoot");
        //PlaySound("Player_WalkLeftFoot");
        //        soundController.PlaySound("Player_WalkLeftFoot");
    }
    

    /// <summary>
    /// Plays the right foot's walking audio clip at the player's location
    /// <summary>
    public void WalkRightFoot()
    {
        //PlaySound("Player_WalkRightFoot");
        //        soundController.PlaySound("Player_WalkRightFoot");

        SoundController.instance.PlaySound("Player_WalkRightFoot");
    }

    /// <summary>
    /// Plays the jumping audio clip at the player's location
    /// <summary>
    public void JumpUp()
    {
        //PlaySound("Player_JumpUp");
        //soundController.PlaySound("Player_JumpUp");
        SoundController.instance.PlaySound("Player_JumpUp");
    }

    /*
    void JumpLand() {
       src.clip = jumpLand;
       src.Play();
    }
    */

    

    /// <summary>
    /// Plays the swinging audio clip at the player's location
    /// <summary>
    public void Swing()
    {
        SoundController.instance.PlaySound("Player_Swing");
        //PlaySound("Player_Swing");
        //soundController.PlaySound("Player_Swing");
    }

    /// <summary>
    /// Plays the damaged audio clip at the player's location
    /// <summary>
    public void Damaged()
    {
        SoundController.instance.PlaySound("Player_Damaged");
        //PlaySound("Player_Damaged");
        //soundController.PlaySound("Player_Damaged");
    }
	
	/// <summary>
    /// Plays the death audio clip at the player's location
    /// <summary>
    public void Died()
    {
        SoundController.instance.PlaySound("Player_Die");
        //PlaySound("Player_Died");
        //soundController.PlaySound("Player_Die");
    }

}
