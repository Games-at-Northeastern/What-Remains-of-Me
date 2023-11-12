using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class ResetAtlasCamInBossRoom : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera VirtualCam;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            VirtualCam.Follow = collision.gameObject.transform;
        }
    }

}
