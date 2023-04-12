using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTeleport : MonoBehaviour
{

    [SerializeField] Transform teleportLocation;
	//these two are used to get the script for the player's sound effects
	private GameObject sfx_holder;
	private PlayerSFX sfx;
	
	void Awake (){
		//Get the objects needed for the sound effects.
		sfx_holder = GameObject.Find("PlayerGraphics");
		sfx = sfx_holder.GetComponent<PlayerSFX>();
	}


    void OnTriggerEnter2D (Collider2D other)
     {
         if (other.gameObject.tag == "Player")
         {
            other.transform.position = teleportLocation.position;
			//play the player death sound using PlayerSFX
			sfx.Died();
         }
     }
}
