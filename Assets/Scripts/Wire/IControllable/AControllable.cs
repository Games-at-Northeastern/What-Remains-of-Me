using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
///     Abstract class which gives Controllable classes certain utilities,
///     such as a general implementation of the GainEnergy() and LoseEnergy()
///     methods, and a representation of energy.
/// </summary>
public abstract class AControllable : MonoBehaviour, IControllable, IDataPersistence
{
    [SerializeField] protected float cleanEnergy;
    [SerializeField] protected float maxCharge;
    [SerializeField] protected float virus;

    public string uniqueID;

    // Event to be triggered whenever the virus amount changes,
    // sending the current percentage of energy that is virus as a value between 0 and 1
    public UnityEvent<float> OnVirusChange;

    public UnityEvent<float> OnEnergyChange;

    private EnergyManager energyManager;

    protected float totalEnergy {
        get => cleanEnergy + virus;
    }


    private void Start() => energyManager = PlayerRef.PlayerManager.EnergyManager;

    /// <summary>
    ///     This controllable gains the given amount of energy and takes it from the player health.
    ///     <param name="amount"> float amount of energy for this controllable to gain </param>
    /// </summary>
    public void GainEnergy(float amount)
    {
        if (energyManager is null) {
            energyManager = PlayerRef.PlayerManager.EnergyManager;
        }

        if (amount <= 0 || totalEnergy >= maxCharge || energyManager.Battery <= 1f) {
            return;
        }
        float totalEnergyBefore = totalEnergy;

        // this is the cause of the outlet's never filling up to full
        amount = Mathf.Min(amount, maxCharge - totalEnergy);
        amount = Mathf.Min(amount, energyManager.Battery - 1f);

        float virusProportion = energyManager.Virus / energyManager.Battery;

        energyManager.Battery -= amount;
        energyManager.Virus -= amount * virusProportion;

        cleanEnergy += amount * (1f - virusProportion);
        virus += amount * virusProportion;
        EnsureNoNegativeEnergy();
        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy - totalEnergyBefore);

        //Debug.Log("battery: " + cleanEnergy + " clean energy units, " + virus + " virus units.");
        //Debug.Log("player: " + (playerInfo.battery - playerInfo.virus) + " clean energy units, " + playerInfo.virus + " virus units");
    }

    /// <summary>
    ///     This controllable loses the given amount of energy and gives it to the player health.
    ///     <param name="amount"> float amount of energy for this controllable to lose </param>
    /// </summary>
    public void LoseEnergy(float amount)
    {
        if (energyManager is null) {
            energyManager = PlayerRef.PlayerManager.EnergyManager;
        }

        if (amount <= 0 || totalEnergy <= 0 || energyManager.Battery >= energyManager.MaxBattery) {
            return;
        }
        float totalEnergyBefore = totalEnergy;

        amount = Mathf.Min(amount, totalEnergy);
        amount = Mathf.Min(amount, energyManager.MaxBattery - energyManager.Battery);

        float virusProportion = virus / totalEnergy;

        energyManager.Battery += amount;
        energyManager.Virus += amount * virusProportion;

        cleanEnergy -= amount * (1f - virusProportion);
        virus -= amount * virusProportion;
        EnsureNoNegativeEnergy();
        VirusChange(virus / totalEnergy);
        EnergyChange(totalEnergy - totalEnergyBefore);

        //Debug.Log("battery: " + cleanEnergy + " clean energy units, " + virus + " virus units.");
        //Debug.Log("player: " + (playerInfo.battery - playerInfo.virus) + " clean energy units, " + playerInfo.virus + " virus units");
    }

    public void LoadPlayerData(PlayerData playerData)
    {
        //No player data to load for Acontrollanble
    }
    public void LoadLevelData(LevelData levelData)
    {
        if (!checkForUniqueIDScript()) {
            return;
        }

        if (levelData == null) {
            Debug.Log($"LodeLevelData: 'levelData' is null on {gameObject.name}.");
            return;
        }

        if (!levelData.outletCleanEnergy.TryGetValue(uniqueID, out float savedCleanEnergy)) {
            Debug.LogError("outletCleanEnergy for " + gameObject + " could not be obtained");
            return;
        }
        cleanEnergy = savedCleanEnergy;

        if (!levelData.outletVirusEnergy.TryGetValue(uniqueID, out float savedVirusEnergy)) {
            Debug.LogError("outletVirusEnergy for " + gameObject + " could not be obtained");
            return;
        }
        if (!levelData.outletMaxEnergy.TryGetValue(uniqueID, out float savedMaxEnergy)) {
            Debug.LogError("outletMaxEnergy for " + gameObject + " could not be obtained");
            return;
        }


        virus = savedVirusEnergy;
        maxCharge = savedMaxEnergy;
    }


    public void SaveData(ref PlayerData playerData, ref LevelData levelData)
    {
        if (!checkForUniqueIDScript()) {
            return;
        }

        //Check for clean outlet energy, if none exists add to dictionary
        if (levelData.outletCleanEnergy.ContainsKey(uniqueID)) {
            levelData.outletCleanEnergy[uniqueID] = cleanEnergy;
        } else {
            levelData.outletCleanEnergy.Add(uniqueID, cleanEnergy);
        }

        //check for virus outlet energy, if none exists add to dictionary
        if (levelData.outletVirusEnergy.ContainsKey(uniqueID)) {
            levelData.outletVirusEnergy[uniqueID] = virus;
        } else {
            levelData.outletVirusEnergy.Add(uniqueID, virus);
        }

        //check for max outlet energy, if none exists add to dictionary
        if (levelData.outletMaxEnergy.ContainsKey(uniqueID)) {
            levelData.outletMaxEnergy[uniqueID] = maxCharge;
        } else {
            levelData.outletMaxEnergy.Add(uniqueID, maxCharge);
        }



    }

    // Gets the clean energy contained within this controllable
    public float GetEnergy() => cleanEnergy;

    // Gets the energy cap of this controllable
    public float GetMaxCharge() => maxCharge;

    /// <summary>
    ///     Energy can go negative when gaining or losing if it overshoots it due to floats. If this is the case, force it to 0
    ///     to prevent bad displaying.
    /// </summary>
    private void EnsureNoNegativeEnergy()
    {
        if (virus < 0) {
            cleanEnergy += virus;
            virus = 0;
        }
        if (cleanEnergy < 0) {
            cleanEnergy = 0;
        }
    }

    // Gets the number of units of virus contained within this controllable
    public float GetVirus() => virus;
    public void VirusChange(float virusPercentage) => OnVirusChange?.Invoke(virusPercentage);
    public void EnergyChange(float totalEnergyAmount) => OnEnergyChange?.Invoke(totalEnergyAmount);

    /// <summary>
    ///     This controllable gains the given amount of energy without taking any from the player health.
    ///     <param name="amount"> float amount of virus for this controllable to gain </param>
    /// </summary>
    public void CreateEnergy(float amount, float virusRatio)
    {
        if (amount <= 0 || totalEnergy >= maxCharge) {
            return;
        }

        amount = Mathf.Min(amount, maxCharge - totalEnergy);

        cleanEnergy += amount * (1f - virusRatio);
        virus += amount * virusRatio;
        EnsureNoNegativeEnergy();
        VirusChange(virus / totalEnergy);
        //EnergyChange(totalEnergy);
    }

    public void SetEnergy(float amount)
    {
        if (amount <= 0 || totalEnergy >= maxCharge) {
            return;
        }

        amount = Mathf.Min(amount, maxCharge - totalEnergy);

        cleanEnergy = amount;
    }

    /// <summary>
    ///     This controllable loses the given amount of energy without giving it to the player.
    ///     <param name="amount"> float amount of virus for this controllable to lose </param>
    /// </summary>
    public void LeakEnergy(float amount)
    {
        if (amount <= 0 || totalEnergy <= 0) {
            return;
        }

        amount = Mathf.Min(amount, totalEnergy);

        float virusProportion = virus / totalEnergy;

        cleanEnergy -= amount * (1f - virusProportion);
        virus -= amount * virusProportion;

        VirusChange(virus / totalEnergy);
        //EnergyChange(totalEnergy);
    }

    /// <summary>
    ///     Returns the percentage of total energy out of max energy that this AControllable has.
    /// </summary>
    public float GetPercentFull() => totalEnergy / maxCharge;

    /// <summary>
    ///     Returns the percentage of total energy in this AControllable which is infected by virus.
    /// </summary>
    public float? GetVirusPercent()
    {
        if (totalEnergy == 0f) {
            return null;
        }
        return virus / totalEnergy;
    }

    /// <summary>
    ///     Can the controllable lose the given amount of energy?
    ///     <param name="amount"> float to compare to player energy </param>
    /// </summary>
    private bool canLoseEnergy(float amount) => totalEnergy >= amount;


    /// <summary>
    ///     Can the player gain the given amount of energy?
    ///     <param name="amount"> float to use as difference from max energy </param>
    /// </summary>
    private bool canGainEnergy(float amount) => totalEnergy + amount <= maxCharge;

    /// <summary>
    ///     Check if uniqueID has been assigned. If it hasnt, the gameobject is missing the UniqueId.cs script
    /// </summary>
    /// <returns>true if uniqueID is not null, false otherwise </returns>
    private bool checkForUniqueIDScript()
    {
        if (uniqueID == null) {
            Debug.Log(gameObject + " data cannot be saved or loaded from this object because it is missing a UniqueId.cs script");
            return false;
        }
        return true;
    }


    public void PropogateEnergyTo(List<AControllable> secondaryControllables)
    {
        int splitEntries = secondaryControllables.Count + 1;
        maxCharge /= splitEntries;
        cleanEnergy /= splitEntries;
        virus /= splitEntries;
        foreach (AControllable controllable in secondaryControllables) {
            controllable.maxCharge = maxCharge;
            controllable.virus = virus;
            controllable.cleanEnergy = cleanEnergy;
        }
    }

    //Debug Control
    /*
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("player virus percentage: " + (playerInfo.virus / playerInfo.battery));
        }
    }
    */
}
