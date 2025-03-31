using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A temporary component that exists to open a door/kill an NPC for scene 3a.
/// It is intentionally designed to be unusable in any other use case except for this one. This is because
/// the concept of "killing NPCs" is a large undertaking and must be done as a separate task.
/// </summary>
public class CutsceneDoorScript : MonoBehaviour
{
    [SerializeField] private AControllable npc;
    [SerializeField] private UnityEvent<bool> toDisable;

    private ControllableDoor door;
    private bool doorOpening = false;

    private void Awake()
    {
        door = GetComponent<ControllableDoor>();

        if (!door)
        {
            throw new MissingReferenceException("Door not found!");
        }

        npc.OnEnergyChange.AddListener(ProcessUpdate);
        npc.OnVirusChange.AddListener(ProcessUpdate);
    }

    private void ProcessUpdate(float _)
    {
        Debug.Log(npc.GetVirus() + " virus " + npc.GetEnergy() + " energy " + npc.GetVirusPercent() + " percent");

        if (npc.GetEnergy() <= 0.5f || npc.GetVirus() >= 50f)
        {
            KillNPC();
            if (!doorOpening)
            {
                doorOpening = true;
                OpenDoor();
            }
        }
        else if (npc.GetVirusPercent() <= 0.4f)
        {
            if (!doorOpening)
            {
                doorOpening = true;
                OpenDoor();
            }
        }
    }

    private void KillNPC()
    {
        npc.gameObject.GetComponent<Animator>().SetInteger("animState", 3);

        toDisable.Invoke(false);
    }

    //cannot get the door to open gradually for some reason. not sure why
    private void OpenDoor()
    {
        float currentTime = 0;
        while (currentTime < 5.0f) //this loop should take 5 seconds
        {
            door.SetEnergy(currentTime * 10.0f);
            currentTime += Time.deltaTime; //maybe Time.deltaTime is off somehow?
        }
    }
}
