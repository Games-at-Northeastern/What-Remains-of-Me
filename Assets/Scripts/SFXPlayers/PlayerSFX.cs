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

    /// <summary>
    /// Plays the ground hit sound effect for when atlas lands from a jump
    /// </summary>
    public void JumpLand(float VolRatio = float.MaxValue) {

        SoundController.instance.PlaySound("Player_JumpLand", VolRatio);
    }

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

    /// <summary>
    /// Plays the laser death sound audio clip at the player's location
    /// <summary>
    public void DiedtoLaser()
    {
        SoundController.instance.PlaySound("Laser_Death_Sound");
    }

    public void DiedToWater()
    {
        SoundController.instance.PlaySound("Player_Die_Water");
    }

}
