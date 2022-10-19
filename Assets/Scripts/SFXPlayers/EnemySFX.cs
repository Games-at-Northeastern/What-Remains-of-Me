using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays the designated audio clip for the enemy's actions
/// <summary>

public class EnemySFX : MonoBehaviour {
   public AudioSource src;
   public AudioClip walk;
   public AudioClip attack;
   public AudioClip overloaded;
   public AudioClip drained;

   /// <summary>
   /// Plays the walking audio clip at the enemy's location
   /// <summary>
   void Walk() {
      src.clip = walk;
      src.Play();
   }

   /// <summary>
   /// Plays the attacking audio clip at the enemy's location
   /// <summary>
   void Attack() {
      src.clip = attack;
      src.Play();
   }

   /// <summary>
   /// Plays the overloaded audio clip at the enemy's location
   /// <summary>
   void OverLoaded() {
      src.clip = overloaded;
      src.Play();
   }
}
