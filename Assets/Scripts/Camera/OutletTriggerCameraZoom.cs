using UnityEngine;
using System.Collections;


public class OutletTriggerCameraZoom : MonoBehaviour

{

    [SerializeField] private Outlet keyOutlet;
    [SerializeField] private GameObject zoomCamera;
    [SerializeField] private BoxCollider2D cameraCollider;

    private bool keyUsed = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraCollider = zoomCamera.GetComponent<BoxCollider2D>();
        cameraCollider.enabled = false;
    
    }



    // Update is called once per frame
    void Update()
    {
        if (KeyOutlet.hasKey && !keyUsed) 
        {
           StartCoroutine(EnableColliderForTime(5f));
           keyUsed = true;
        } 
    }

    // Enables the collider for a certain amount of time
    private IEnumerator EnableColliderForTime(float time) 
    {
        yield return new WaitForSeconds(1f);
        cameraCollider.enabled = true;
        yield return new WaitForSeconds(time);
        cameraCollider.enabled = false;
    }
}
