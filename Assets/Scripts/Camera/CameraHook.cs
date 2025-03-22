using System;
using Cinemachine;
using PlayerController;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFramer : MonoBehaviour
{

    private CinemachineVirtualCamera framingCamera;
    private CinemachineSmoothPath dollyPath;

    private BoxCollider2D frameTrigger;



    // Start is called before the first frame update
    private void Start()
    {
        framingCamera.Priority = -100;
        framingCamera.Follow = FindObjectOfType<PlayerController2D>().transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            framingCamera.Priority = 100;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            framingCamera.Priority = -100;
        }
    }

    //Camera size that would be required to frame the vertical height entirely
    private float CalculateVerticalFrameSize() => frameTrigger.size.y / 2f;

    //Camera size that would be required to frame the vertical entirely
    private float CalculateHorizontalFrameSize() => frameTrigger.size.x / 2f * (9f / 16f);


    private void UpdatePath()
    {
        if (!frameTrigger)
        {
            frameTrigger = GetComponent<BoxCollider2D>();
            framingCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            dollyPath = GetComponentInChildren<CinemachineSmoothPath>();
        }

        if (!dollyPath)
        {
            return;
        }

        dollyPath.transform.localPosition = new Vector3(frameTrigger.offset.x, frameTrigger.offset.y, -10);
        framingCamera.m_Lens.OrthographicSize = Mathf.Max(CalculateVerticalFrameSize(), CalculateHorizontalFrameSize());
        dollyPath.m_Waypoints[0].position = new Vector3(0, 0, -10);
        dollyPath.m_Waypoints[1].position = new Vector3(0, 0, -10);
    }

#if UNITY_EDITOR
    private void OnValidate() => UpdatePath();

#endif
}
