using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays the designated audio clip for the enemy's actions
/// <summary>

public class EnemySFX : MonoBehaviour
{

    [SerializeField] private SoundController soundController;

    /// <summary>
    /// Plays the walking audio clip at the enemy's location
    /// <summary>
    void Walk()
    {
        soundController.PlaySound("Enemy_Walk");
    }

    /// <summary>
    /// Plays the attacking audio clip at the enemy's location
    /// <summary>
    void Attack()
    {
        soundController.PlaySound("Enemy_Attack");
    }

    /// <summary>
    /// Plays the overloaded audio clip at the enemy's location
    /// <summary>
    void OverLoaded()
    {
        soundController.PlaySound("Enemy_Overloaded");
    }
}
