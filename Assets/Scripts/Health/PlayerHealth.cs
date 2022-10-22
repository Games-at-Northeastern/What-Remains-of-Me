using System.Collections;
using System.Collections.Generic;
using SmartScriptableObjects.FloatEvent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps track of, and handles changes in, the player's health (battery).
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;
    bool iframes;
    
    /*
     * sets the PlayerHealth component to this object and gets the 
     * and sets the battery and max battery at run time.
     * 
     * @throw error     if PlayerHealth already exists
     * 
     */
    private void Awake()
    {
    }

    /*
     * deducts the amount of energy given up until 0 for energy transmission use
     * and kills the player if the battery is 0.
     * 
     * @param amount        amount of energy to deduct from the battery
     */
    public void LoseEnergy(float amount)
    {
        playerInfo.battery = Mathf.Clamp(playerInfo.battery - amount, 0, playerInfo.maxBattery);
        if (playerInfo.battery == 0)
        {
            Die();
        }
        OnHealthChanged.Invoke();
    }

    /*
     * adds energy up until the maximum amount that is currently able
     * to be held for use in energy transmission. Kills the player if
     * battery is 0.
     * 
     * 
     * @param amount        amount of energy to add to the battery
     */
    public void GainEnergy(float amount)
    {
        playerInfo.battery = Mathf.Clamp(playerInfo.battery + amount, 0, playerInfo.maxBattery);
        OnHealthChanged.Invoke();
    }

    /*
     * adds the given amount to the virus meter and decreases current battery 
     * value to the new max if battery is full and kills the player if battery
     * is 0
     * 
     * @param amount        amount to add to the virus and deduct from the max
     *                      battery
     */
    public void AddVirus(float amount)
    {
        playerInfo.virus.Value = Mathf.Min(playerInfo.virus.Value + amount / playerInfo.maxVirus, 1f);
        playerInfo.maxBattery -= amount;
        if (playerInfo.maxBattery < playerInfo.battery) { playerInfo.battery = playerInfo.maxBattery; }
        if (playerInfo.battery == 0) { Die(); }
    }

    public void SubtractVirus(float amount)
    {
        playerInfo.virus.Value = Mathf.Max(playerInfo.virus.Value - amount / playerInfo.maxVirus, 0);
        playerInfo.maxBattery += amount;
    }

    /* 
     * Gives the battery amount the player has, from 0 to 1.
     */
    public float GetRelativeBattery()
    {
        return playerInfo.battery / playerInfo.maxBattery;
    }

    /*
     * Can the player transfer this amount of energy to other things?
     * 
     * @param amount     the amount of energy being given
     */
    public bool CanGiveEnergy(float amount)
    {
        return playerInfo.battery >= amount;
    }

    /*
     * Can the player take this amount of energy from other things?
     * 
     * @param amount    the amount of energy being taken
     */
    public bool CanTakeEnergy(float amount)
    {
        return playerInfo.battery <= playerInfo.maxBattery - amount;
    }

    /*
     * Damages the player if there are no Iframes.
     * 
     * @param amount     the amount of damage to be deducted
     */
    public void RequestTakeDamage(float amount)
    {
        if (!iframes)
        {
            TakeDamage(amount);
        }
    }

    /*
     * activates events for taking damage, depletes
     * the energy by the set amount and start IFrames
     */
    private void TakeDamage(float amount)
    {
        OnDamageTaken.Invoke();
        LoseEnergy(amount);
        StartCoroutine(IFrameSequence());
    }

    /*
     * gives IFrames for the specified amount of time by 
     * changing the boolean flag.
     * 
     */
    IEnumerator IFrameSequence()
    {
        iframes = true;
        yield return new WaitForSeconds(playerInfo.iframesTime);
        iframes = false;
    }

    /*
     * reloads the current scene using the Unity Scene Manager
     */
    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
