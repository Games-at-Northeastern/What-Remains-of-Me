using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DetachCameraFromPlayer : MonoBehaviour
{
    [SerializeField] private GameObject GameCamera;
    [SerializeField] private CinemachineVirtualCamera VirtualCam;
    [SerializeField] private Transform bossRoomCameraCenter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            VirtualCam.Follow = collision.gameObject.transform;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            VirtualCam.Follow = collision.gameObject.transform;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            VirtualCam.Follow = bossRoomCameraCenter;
            GameCamera.transform.position = bossRoomCameraCenter.position; //hardcoded :P
        }
    }


}
