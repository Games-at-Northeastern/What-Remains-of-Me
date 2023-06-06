using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DetachCameraFromPlayer : MonoBehaviour
{
    [SerializeField] private GameObject GameCamera;
    [SerializeField] private CinemachineVirtualCamera VirtualCam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            VirtualCam.Follow = null;
            GameCamera.transform.position = new Vector3(46.5f, 6.5f, 0f); //hardcoded :P
        }
    }
}
