using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// why is this just for spikes? TODO, abstract this.
public class SpikeTeleport : MonoBehaviour
{

    [SerializeField] Transform teleportLocation;
    //these two are used to get the script for the player's sound effects
    private GameObject sfx_holder;
    private PlayerSFX sfx;


    [SerializeField] private ParticleSystem deathParticles;

    private GameObject objectToTeleport;

    private void Awake()
    {
        //Get the objects needed for the sound effects.
        sfx_holder = GameObject.Find("PlayerGraphics");
        sfx = sfx_holder.GetComponent<PlayerSFX>();
    }


    // TODO! This triggers 4 times per every collision with the player due to the multiple colliders on them.
    // That's really weird. Why?
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PerformDeath(other.gameObject);
        }
    }

    // exposed so that killing things is just more convenient
    public void PerformDeath(GameObject target)
    {
        objectToTeleport = target.gameObject;
        deathParticles.gameObject.transform.position = objectToTeleport.transform.position;
        deathParticles.Clear();
        deathParticles.Play();
        objectToTeleport.SetActive(false);

        Invoke(nameof(TeleportPlayer), deathParticles.main.duration);
    }

    private void TeleportPlayer()
    {


        // TODO: Remove this once CheckpointManagers have been placed in every scene. Until then,
        // keep this check to make sure no current scenes break
        if (FindObjectOfType<CheckpointManager>() == null)
        {

            objectToTeleport.transform.position = teleportLocation.position;
        }

        objectToTeleport.SetActive(true);
        LevelManager.Instance.PlayerReset();
        InkDialogueVariables.deathCount++;

        //play the player death sound using PlayerSFX
        sfx.Died();
    }
}


