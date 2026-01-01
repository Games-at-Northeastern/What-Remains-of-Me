using UnityEngine;
/// <summary>
///     Class attached to an outlet to an outlet that has a controllable with the purpose of filtering players through an
///     area by detecting the power they have.
/// </summary>
/// NOTE:Example currently found in 1.5.2 if you want to see its use case.
public class OutletDoorOverrider : MonoBehaviour
{
    [SerializeField]
    //Done as percenteages, so the percent the a player must have(or lower) to pass through
    [Range(0.0F, 100.0F)]
    private float threashHold = 15f;
    //Do they need a connection to the outlet to start the sequence?
    [SerializeField]
    private bool needConnection;
    //Among of energy added to the door per second after the threasehold has been hit
    [SerializeField]
    private float chargeIncreasePerSecond;
    //A delay before the charge starts occuring
    [SerializeField]
    private float timeWaited = 2f;
    [SerializeField]
    private InkDialogueAmbientTrigger turnOnTrigger;
    private PlayerHealth health;
    //What controllable is this overriding?
    private AControllable overridingDoor;
    //Has the automatic rise started?
    private bool sequenceStarted;
    private float timer;

    //Player values linked up to check Atlas's status
    private WireThrower wireThrower;
    // Start is called before the first frame update
    private void Start()
    {
        health = FindObjectOfType<PlayerHealth>();
        wireThrower = FindObjectOfType<WireThrower>();
        overridingDoor = GetComponent<Outlet>().controlled;
    }

    // Update is called once per frame
    private void Update()
    {
        //Do we need a connection?s
        bool canStart = true;
        if (needConnection) {
            canStart = GetComponent<Outlet>() == wireThrower.ConnectedOutlet;
        }

        //Have we hit the threashold and can we start the sequence?
        if (EnergyManager.Instance.Battery < threashHold && !sequenceStarted && canStart) {
            sequenceStarted = true;
            wireThrower.DisconnectWire();
            tag = "Untagged";
            GetComponent<Collider2D>().enabled = false;

        }
        //Have we hit the threasehold and just need to wait for the delay?
        else if (sequenceStarted && timer < timeWaited) {
            overridingDoor.LeakEnergy(Time.deltaTime * 100f);
            timer = Mathf.Min(Time.deltaTime + timer, timeWaited);
        }
        //Can we start charging up the door?
        else if (sequenceStarted && overridingDoor.GetEnergy() < overridingDoor.GetMaxCharge()) {
            overridingDoor.CreateEnergy(chargeIncreasePerSecond * Time.deltaTime, 0f);
            if (turnOnTrigger != null) {
                turnOnTrigger.TurnOffAdditionalTriggerRequirement();
            }
        }

    }
}
