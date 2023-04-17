using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField]
    float zoomFactor = 1.0f;

    [SerializeField]
    float zoomSpeed = 5.0f;

    private float originalSize = 0f;

    private Camera thisCamera;

// Use this for initialization
    void Start()
    {
        thisCamera = GetComponent<Camera>();
        originalSize = thisCamera.orthographicSize;
    }

// Update is called once per frame
    void Update()
    {
        float targetSize = originalSize * zoomFactor;
        if (targetSize != thisCamera.orthographicSize)
        {
            thisCamera.orthographicSize = Mathf.Lerp(thisCamera.orthographicSize,
                targetSize, Time.deltaTime * zoomSpeed);
        }
    }

    void SetZoom(float zoomFactor)
    {
        this.zoomFactor = zoomFactor;
    }
}
