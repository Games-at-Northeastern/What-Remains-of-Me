using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraSystemManager : MonoBehaviour
{
    #region Cameras (Cameras in a system must be children of the system object)
    [SerializeField] private GameObject[] cameraSystem;
    public bool systemFlag = false;
    public bool alreadyActivated = false;
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        systemFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(systemFlag && !alreadyActivated)
        {
            SetAllCamerasActive();
            Debug.Log("Camera System Activated");
        }
    }

    private void SetAllCamerasActive()
    {
        int i = 1;
        foreach (GameObject camera in cameraSystem)
        {
            Debug.Log("Set camera " + i + " cone light to red");
            Debug.Log(camera.GetComponent<InteractOnRayDetect>().detectCone);
            camera.SetActive(false); //temporary because camera cone lights refuse to turn red even though they are properly being referenced because of course it doesn't work when it should :)
            camera.GetComponent<InteractOnRayDetect>().detectCone.color = new Color(1,0,0,1);
            i++;
        }
        alreadyActivated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            systemFlag = false;

            int i = 1;
            foreach (GameObject camera in cameraSystem)
            {
                camera.SetActive(true); //temporary because camera cone lights refuse to turn red even though they are properly being referenced because of course it doesn't work when it should :)
                camera.GetComponent<InteractOnRayDetect>().detectCone.color = new Color(0,1,0,1);
                i++;
            }

            alreadyActivated = false;
        }
    }
}
