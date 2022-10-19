using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Plays the designated audio clip for the player's actions
/// <summary>
public class PlayerSFX : MonoBehaviour {

   public AudioSource src;
   public AudioClip walkLeftFoot;
   public AudioClip walkRightFoot;
   public AudioClip jumpUp;
   // public AudioClip jumpLand;
   public AudioClip swing;
   public AudioClip damaged;

   /// <summary>
   /// Plays the left foot's walking audio clip at the player's location
   /// <summary>
   void WalkLeftFoot() {
      src.clip = walkLeftFoot;
      src.Play();
   }

   /// <summary>
   /// Plays the right foot's walking audio clip at the player's location
   /// <summary>
   void WalkRightFoot() {
      src.clip = walkRightFoot;
      src.Play();
   }

   /// <summary>
   /// Plays the jumping audio clip at the player's location
   /// <summary>
   void JumpUp() {
      src.clip = jumpUp;
      src.Play();
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
   void Swing() {
      src.clip = swing;
      src.Play();
   }

   /// <summary>
   /// Plays the damaged audio clip at the player's location
   /// <summary>
   void Damaged() {
      src.clip = damaged;
      src.Play();
   }

}
