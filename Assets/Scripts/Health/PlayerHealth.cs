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
    [Header("Player Stats")]
    [SerializeField] float initBattery;
    [SerializeField] float maxBattery;
    [SerializeField] float iframesTime;
    [SerializeField] private float _maxVirus;
    
    [Header("Dependency Injection")] 
    [SerializeField] private FloatReactivePropertySO _virusValSO;

    static float Battery;
    static float MaxBattery;
    public static PlayerHealth instance { get; private set; }
    
    [Header("Player Events")]
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;
    bool iframes;

    private IFloatReactiveProperty _virusVal;

    /*
     * sets the PlayerHealth component to this object and gets the 
     * and sets the battery and max battery at run time.
     * 
     * @throw error     if PlayerHealth already exists
     * 
     */
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Battery = initBattery;
            MaxBattery = maxBattery;
            _virusVal = _virusValSO;
        }
        else
        {
            Debug.LogError("Instance already exists for PlayerHealth");
        }
    }

    /*
     * deducts the amount of energy given up until 0 for energy transmission use
     * and kills the player if the battery is 0.
     * 
     * @param amount        amount of energy to deduct from the battery
     */
    public void LoseEnergy(float amount)
    {
        Battery = Mathf.Clamp(Battery - amount, 0, maxBattery);
        if (Battery == 0)
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
        Battery = Mathf.Clamp(Battery + amount, 0, maxBattery);
        if (Battery == 0)
        {
            Die();
        }
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
        _virusVal.Value = Mathf.Min(_virusVal.Value + amount / _maxVirus, 1f);
        MaxBattery -= amount;
        if (MaxBattery < Battery) { Battery = MaxBattery; }
        if (Battery == 0) { Die(); }
    }

    public void SubtractVirus(float amount)
    {
        _virusVal.Value = Mathf.Max(_virusVal.Value - amount / _maxVirus, 0);
        MaxBattery += amount;
    }

    /* 
     * Gives the battery amount the player has, from 0 to 1.
     */
    public static float GetRelativeBattery()
    {
        return Battery / MaxBattery;
    }

    /*
     * Can the player transfer this amount of energy to other things?
     * 
     * @param amount     the amount of energy being given
     */
    public static bool CanGiveEnergy(float amount)
    {
        return Battery >= amount;
    }

    /*
     * Can the player take this amount of energy from other things?
     * 
     * @param amount    the amount of energy being taken
     */
    public static bool CanTakeEnergy(float amount)
    {
        return Battery <= MaxBattery - amount;
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
        yield return new WaitForSeconds(iframesTime);
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
