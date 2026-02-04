using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
///     Keeps track of, and handles changes in, the player's health (battery).
/// </summary>
public class PlayerHealth : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject warning;
    public PlayerInfo playerInfo;
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDamageTaken;
    private EnergyManager energyManager;
    private bool iframes;

    /*
     * sets the PlayerHealth component to object and gets the
     * and sets the battery and max battery at run time.
     *
     * @throw error     if PlayerHealth already exists
     *
     */

    private void Start() => energyManager = PlayerManager.Instance.EnergyManager;

    private void Update()
    {
        // TODO : This should really be done with listeners/events - ideally, this would actually be
        // replaced with an AControllable component, but that would require some rework of the AControllable class
        // to not directly reference the PlayerInfo

        // check if player has depleted all health (with some float forgiveness)
        if (energyManager.Battery <= 0.01) {
            EnergyDepletedDeath();
        }
        // check if player has reached minimum health (the player cannot drain any more health)
        else if (energyManager.Battery <= 1f) {
            EnergyDepletionWarning();
        } else {
            warning.GetComponent<WarningController>().StopLowHealthWarning();
        }

        // check if the player has reached 100% virus (with some float forgiveness)
        if (energyManager.Virus >= energyManager.MaxVirus - 0.01) {
            VirusFullDeath();
        }
        // check if the player has any virus
        else if (energyManager.Virus >= 0.01) {
            VirusWarning();
        } else {
            warning.GetComponent<WarningController>().StopLightBlinking();
            warning.GetComponent<WarningController>().ResetVirus();
        }


    }

    public void LoadPlayerData(PlayerData playerData)
    {
        energyManager.Battery = playerData.batteryPercentage * energyManager.MaxBattery;
        energyManager.Virus = playerData.virusPercentage * energyManager.MaxVirus;
    }
    public void LoadLevelData(LevelData levelData)
    {
        //No level data to load for Player Health
    }
    public void SaveData(ref PlayerData playerData, ref LevelData levelData)
    {
        playerData.batteryPercentage = energyManager.BatteryPercentage;
        playerData.virusPercentage = energyManager.VirusPercentage;

    }

    // TODO : Should there be some kind of scene resetting that is triggered by this? I.e. platforms, enemies, etc.?

    /// <summary>
    ///     Represents any necessary steps to handle the player death when they hold their max Virus amount.
    /// </summary>
    private void VirusFullDeath()
    {
        Debug.Log("Death from virus full");
        energyManager.Virus = 0f;
        LevelManager.PlayerDeath();
    }

    /// <summary>
    ///     Represents any necessary steps to handle the player death when their battery level reaches 0 from depletion.
    /// </summary>
    private void EnergyDepletedDeath()
    {
        Debug.Log("Death from energy depleted");
        LevelManager.PlayerDeath();
    }

    /// <summary>
    ///     Represents any necessary steps to handle the player death when their battery level reaches 0 from taking damage.
    /// </summary>
    private void EnergyDamageDeath() => Debug.Log("Death from energy empty upon damage");


    // Methods for to triggering audiovisual warnings for when the player is about to die

    /// <summary>
    ///     Triggers the virus warning (blinking headlight) at high virus percentages.
    ///     Additionally, updates eye and light color to match virus percentage.
    /// </summary>
    private void VirusWarning()
    {
        warning.GetComponent<WarningController>().VirusControl(energyManager.Virus / energyManager.MaxVirus);
        if (energyManager.Virus >= energyManager.MaxVirus * 0.8f) {
            Debug.Log("Virus Overload Warning");
            warning.GetComponent<WarningController>().StartLightBlinking();
        } else {
            warning.GetComponent<WarningController>().StopLightBlinking();
        }
    }


    /// <summary>
    ///     Triggers warnings for reaching minimum energy
    /// </summary>
    private void EnergyDepletionWarning() =>
        // Debug.Log("Energy Depletion Warning");
        warning.GetComponent<WarningController>().StartLowHealthWarning();


    // -------------- Are these being used? Don't think so --------------

    /*
     * deducts the amount of energy given up until 0 for energy transmission use
     * and kills the player if the battery is 0.
     *
     * @param amount        amount of energy to deduct from the battery
     */
    public void LoseEnergy(float amount)
    {
        energyManager.Battery -= amount;
        if (energyManager.BatteryPercentage <= 0f) {
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
        energyManager.Battery += amount;
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
        energyManager.Virus += amount;
        // playerInfo.maxBattery -= amount;
        if (energyManager.BatteryPercentage <= 0f || energyManager.VirusPercentage >= 0.99f) {
            Die();
        }
    }

    public void SubtractVirus(float amount) => energyManager.Virus -= -amount;
    // playerInfo.maxBattery += amount;
    /*
     * Gives the battery amount the player has, from 0 to 1.
     */
    public float GetRelativeBattery() => energyManager.BatteryPercentage;

    /*
     * Can the player transfer amount of energy to other things?
     *
     * @param amount     the amount of energy being given
     */
    public bool CanGiveEnergy(float amount) => energyManager.Battery >= amount;

    /*
     * Can the player take amount of energy from other things?
     *
     * @param amount    the amount of energy being taken
     */
    public bool CanTakeEnergy(float amount) => energyManager.Battery + energyManager.Virus <=
        energyManager.MaxBattery - amount;

    /*
     * Damages the player if there are no Iframes.
     *
     * @param amount     the amount of damage to be deducted
     */
    public void RequestTakeDamage(float amount)
    {
        if (!iframes) {
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
    private IEnumerator IFrameSequence()
    {
        iframes = true;
        yield return new WaitForSeconds(playerInfo.iframesTime);
        iframes = false;
    }

    /*
     * reloads the current scene using the Unity Scene Manager
     */
    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        InkDialogueVariables.deathCount++;
    }
}
