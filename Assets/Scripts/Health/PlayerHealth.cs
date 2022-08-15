using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps track of, and handles changes in, the player's health (battery).
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float initBattery;
    [SerializeField] float maxBattery;
    [SerializeField] float virus;
    [SerializeField] float iframesTime;
    static float Battery;
    static float MaxBattery;
    public static PlayerHealth instance { get; private set; }
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;
    bool iframes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Battery = initBattery;
            MaxBattery = maxBattery;
        }
        else
        {
            Debug.LogError("Instance already exists for PlayerHealth");
        }
    }

    /// <summary>
    /// Loses the given amount of energy, not allowing the energy to go above 0 or
    /// the max amount. If you want to avoid death, this function should not be
    /// called unless it is known that this amount of energy CAN be lost.
    /// This function should be called for the sake of energy transmission,
    /// not taking damage.
    /// </summary>
    public void LoseEnergy(float amount)
    {
        Battery = Mathf.Clamp(Battery - amount, 0, maxBattery);
        if (Battery == 0)
        {
            Die();
        }
        OnHealthChanged.Invoke();
    }


    /// <summary>
    /// Gains the given amount of energy, not allowing the energy to go above 0 or
    /// the max amount. If you don't want max energy, this function should
    /// not be called unless it is known that this amount of energy CAN be gained.
    /// This function should be called for the sake of energy transmission,
    /// not taking damage.
    /// </summary>
    public void GainEnergy(float amount)
    {
        Battery = Mathf.Clamp(Battery + amount, 0, maxBattery);
        if (Battery == 0)
        {
            Die();
        }
        OnHealthChanged.Invoke();
    }

    public void AddVirus(float amount)
    {
        virus += amount;
        MaxBattery -= amount;
        if (MaxBattery < Battery) { Battery = MaxBattery; }
        if (Battery == 0) { Die(); }
    }

    public void SubtractVirus(float amount)
    {
        virus -= amount;
        MaxBattery += amount;
    }

    /// <summary>
    /// Gives the battery amount the player has, from 0 to 1.
    /// </summary>
    public static float GetRelativeBattery()
    {
        return Battery / MaxBattery;
    }

    /// <summary>
    /// Can the player transfer this amount of energy to other things?
    /// </summary>
    public static bool CanGiveEnergy(float amount)
    {
        return Battery >= amount;
    }

    /// <summary>
    /// Can the player take this amount of energy from other things?
    /// </summary>
    public static bool CanTakeEnergy(float amount)
    {
        return Battery <= MaxBattery - amount;
    }

    /// <summary>
    /// Makes the player take the given amount of damage
    /// if they are not immune right now for some reason.
    /// </summary>
    public void RequestTakeDamage(float amount)
    {
        if (!iframes)
        {
            TakeDamage(amount);
        }
    }

    /// <summary>
    /// Makes the player take the given amount of damage.
    /// </summary>
    private void TakeDamage(float amount)
    {
        OnDamageTaken.Invoke();
        LoseEnergy(amount);
        StartCoroutine(IFrameSequence());
    }

    /// <summary>
    /// Gives the player IFrames for a temporary amount of time. 
    /// </summary>
    IEnumerator IFrameSequence()
    {
        iframes = true;
        yield return new WaitForSeconds(iframesTime);
        iframes = false;
    }

    /// <summary>
    /// Conduct the death sequence.
    /// </summary>
    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
