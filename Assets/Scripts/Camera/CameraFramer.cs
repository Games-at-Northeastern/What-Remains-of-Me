using System;

using PlayerController;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraFramer : MonoBehaviour
{
    private enum FramingMode
    {
        FrameEntireScreen,
        TrackHorizontally,
        TrackVertically
    }

    [SerializeField]
    private FramingMode framingMode;

    private Unity.Cinemachine.CinemachineCamera framingCamera;
    private SplineContainer dollyPath;

    private BoxCollider2D frameTrigger;
    private PlayerController2D player;

    private void Awake() {
        frameTrigger = GetComponent<BoxCollider2D>();
        framingCamera = GetComponentInChildren<Unity.Cinemachine.CinemachineCamera>();
        dollyPath = GetComponentInChildren<SplineContainer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
    }

    private void Start() {
        if (framingCamera != null) {
            framingCamera.Priority = -100;
            if (player != null) {
                framingCamera.Follow = player.transform;
            }
            else {
                Debug.LogWarning("Player not found!");
            }
        }
        else {
            Debug.LogError("CinemachineVirtualCamera not found!");
        }
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
    //TO DO: This assumes that we're working with a standard 16 by 9 aspect ratio, this should be changed later to adapt to whatever aspect ratio we have
    private float CalculateHorizontalFrameSize() => frameTrigger.size.x / 2f * (9f / 16f);


    private void UpdatePath()
    {
        if (!frameTrigger)
        {
            frameTrigger = GetComponent<BoxCollider2D>();
            framingCamera = GetComponentInChildren<Unity.Cinemachine.CinemachineCamera>();
            dollyPath = GetComponentInChildren<SplineContainer>();
        }

        if (!dollyPath)
        {
            return;
        }

        dollyPath.transform.localPosition = new Vector3(frameTrigger.offset.x, frameTrigger.offset.y, -10);

        while (dollyPath.Spline.Count < 2)
        {
            dollyPath.Spline.Add(new BezierKnot(float3.zero));
        }

        switch (framingMode)
        {
            case FramingMode.FrameEntireScreen:
                framingCamera.Lens.OrthographicSize = Mathf.Max(CalculateVerticalFrameSize(), CalculateHorizontalFrameSize());
                SetKnotPositions(Vector3.zero, Vector3.zero);
                break;
            case FramingMode.TrackHorizontally:
                framingCamera.Lens.OrthographicSize = CalculateVerticalFrameSize();
                SetKnotPositions(
                    new Vector3(-frameTrigger.size.x / 2f, 0, -10),
                    new Vector3(frameTrigger.size.x / 2f, 0, -10));
                break;
            case FramingMode.TrackVertically:
                framingCamera.Lens.OrthographicSize = CalculateHorizontalFrameSize();
                SetKnotPositions(
                    new Vector3(0, -CalculateVerticalFrameSize(), -10),
                    new Vector3(0, CalculateVerticalFrameSize(), -10));
                break;
            default:
                break;
        }
    }

    private void SetKnotPositions(Vector3 first, Vector3 second)
    {
        dollyPath.Spline.SetKnot(0, new BezierKnot((float3)first));
        dollyPath.Spline.SetKnot(1, new BezierKnot((float3)second));
    }

#if UNITY_EDITOR
    private void OnValidate() => UpdatePath();

#endif
}
