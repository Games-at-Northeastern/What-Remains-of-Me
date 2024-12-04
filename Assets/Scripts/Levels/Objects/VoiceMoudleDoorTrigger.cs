using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceMoudleDoorTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]  private Animator anim;
    
    [SerializeField]  private string openDoorTrigger = "VoiceBox";

    public BoxCollider2D detectionArea; //  BoxCollider2D
    private bool isPlayerInArea = false;

    private void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

      

    }   
        void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = true;
            OpenDoor();
        }
    }


    void OpenDoor()
    {
        if (anim != null)
        {
            anim.SetTrigger(openDoorTrigger);
        }
    }

}
