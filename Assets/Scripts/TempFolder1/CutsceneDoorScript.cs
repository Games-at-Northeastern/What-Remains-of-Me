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
            OpenDoor();
        }
        else if (npc.GetVirusPercent() <= 0.4f)
        {
            OpenDoor();
        }
    }

    private void KillNPC()
    {
        npc.gameObject.GetComponent<Animator>().SetInteger("animState", 3);

        toDisable.Invoke(false);
    }

    private void OpenDoor() => door.GainEnergy(50);
}
