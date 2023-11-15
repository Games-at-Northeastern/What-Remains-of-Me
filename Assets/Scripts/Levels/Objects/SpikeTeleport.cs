using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {


            objectToTeleport = other.gameObject;
            deathParticles.gameObject.transform.position = objectToTeleport.transform.position;
            deathParticles.Clear();
            deathParticles.Play();
            objectToTeleport.SetActive(false);

            Invoke("TeleportPlayer", deathParticles.main.duration);

        }
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


