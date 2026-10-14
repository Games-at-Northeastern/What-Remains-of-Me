using System.Collections;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class DeathZone : MonoBehaviour
{
    //Should this death zone instantly kill the player, or just deal damage 
    public DeathZoneType zoneType;
    //If this is a pit, indicate how long the player falls before death 
    public float pitDelay;
    //If this should deal damage to the batter instead, how much 
    public float batteryDamage;

    private bool canDieToPit;
    //Collider and death zone for this class 
    private BoxCollider2D collider;
    private EnergyManager energyManager;
    //Player health to use in internal functions 
    private PlayerHealth playerHealthScript;

    private void Awake()
    {
        canDieToPit = true;
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        playerHealthScript = GameObject.Find("Player").GetComponentInChildren(typeof(PlayerHealth)) as PlayerHealth;
    }

    private void Start() => energyManager = PlayerRef.PlayerManager.EnergyManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            switch (zoneType) {
                case DeathZoneType.instakill:
                    playerHealthScript.LoseEnergy(energyManager.MaxBattery);
                    break;
                case DeathZoneType.damage:
                    playerHealthScript.LoseEnergy(batteryDamage);
                    break;
                case DeathZoneType.pit:
                    if (canDieToPit)
                        StartCoroutine(pitDeath());
                    break;
            }
        }
    }

    //Simulates a fall before death 
    private IEnumerator pitDeath()
    {
        canDieToPit = false;
        yield return new WaitForSecondsRealtime(pitDelay);
        playerHealthScript.LoseEnergy(energyManager.MaxBattery);
        canDieToPit = true;
    }
}

public enum DeathZoneType
{
    instakill,
    damage,
    pit
}
