using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Plays the designated audio clip for the player's actions
/// <summary>
public class PlayerSFX : MonoBehaviour
{

    [SerializeField] private SoundController soundController;

    /// <summary>
    /// Plays the left foot's walking audio clip at the player's location
    /// <summary>
    void WalkLeftFoot()
    {
        soundController.PlaySound("Player_WalkLeftFoot");
    }

    /// <summary>
    /// Plays the right foot's walking audio clip at the player's location
    /// <summary>
    void WalkRightFoot()
    {
        soundController.PlaySound("Player_WalkRightFoot");
    }

    /// <summary>
    /// Plays the jumping audio clip at the player's location
    /// <summary>
    void JumpUp()
    {
        soundController.PlaySound("Player_JumpUp");
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
    void Swing()
    {
        soundController.PlaySound("Player_Swing");
    }

    /// <summary>
    /// Plays the damaged audio clip at the player's location
    /// <summary>
    void Damaged()
    {
        soundController.PlaySound("Player_Damaged");
    }
	
	/// <summary>
    /// Plays the death audio clip at the player's location
    /// <summary>
    void Died()
    {
        soundController.PlaySound("Player_Die");
    }

}
