using UnityEngine;

public class OutletTriggerCameraZoom : MonoBehaviour

{

    [SerializeField] private Outlet keyOutlet;
    [SerializeField] private GameObject zoomCamera;
    [SerializeField] private BoxCollider2D cameraCollider;
    private bool connected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        cameraCollider = zoomCamera.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        connected = keyOutlet.isConnected;
        cameraCollider.enabled = connected;
    }
}
