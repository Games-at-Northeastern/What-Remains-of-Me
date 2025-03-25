using UnityEngine;

public class OutletDoorOverrider : MonoBehaviour
{
    [SerializeField]
    [Range(0.0F, 100.0F)]
    private float threashHold = 15f;
    [SerializeField]
    private bool needConnection;
    private bool sequenceStarted = false;
    private PlayerHealth health;
    private float timer = 0;
    [SerializeField]
    private float chargeIncreasePerSecond;
    [SerializeField]
    private float timeWaited = 2f;
    private AControllable overridingDoor;
    private WireThrower wireThrower;
    [SerializeField] InkDialogueAmbientTrigger turnOnTrigger;
    // Start is called before the first frame update
    void Start()
    {
        health = FindObjectOfType<PlayerHealth>();
        wireThrower = FindObjectOfType<WireThrower>();
        overridingDoor = GetComponent<Outlet>().controlled;
    }

    // Update is called once per frame
    private void Update()
    {
        bool canStart = true;
        if (needConnection)
        {
            canStart = GetComponent<Outlet>() == wireThrower.ConnectedOutlet;
        }


        if (health.playerInfo.battery < threashHold && !sequenceStarted && canStart)
        {
            sequenceStarted = true;
            wireThrower.DisconnectWire();
            tag = "Untagged";
            GetComponent<Collider2D>().enabled = false;

        }
        else if (sequenceStarted && timer < timeWaited)
        {
            overridingDoor.LeakEnergy(Time.deltaTime * 100f);
            timer = Mathf.Min(Time.deltaTime + timer, timeWaited);
        }
        else if (sequenceStarted && overridingDoor.GetEnergy() < overridingDoor.GetMaxCharge())
        {
            overridingDoor.CreateEnergy(chargeIncreasePerSecond * Time.deltaTime, 0f);
            if (turnOnTrigger != null)
            {
                turnOnTrigger.TurnOffAdditionalTriggerRequirement();
            }
        }

    }
}
