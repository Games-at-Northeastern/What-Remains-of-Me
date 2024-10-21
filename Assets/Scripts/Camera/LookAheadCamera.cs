using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LookAheadCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer transposer;
    [SerializeField] private PlayerController.PlayerController2D player;

    private Vector3 currentLookAhead;

    private float maxLookAheadDistance = 3.5f; //max distance the camera can look ahead.
    private float lookAheadSpeed = 2f; //speed of camera movement.
    private Camera thisCamera;

    void Start()
    {
        //get the framing transposer component from the virtual camera
        transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentLookAhead = transposer.m_TrackedObjectOffset;
        thisCamera = Camera.main;
    }

    void Update()
    {
        //get the mouse position in the world
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        //get the center of the screen in world coordinates
        Vector3 screenCenter = thisCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        //calculate the offset based on how far the mouse is from the center of the screen
        Vector3 offset = mousePos - screenCenter;

        //clamp the offset to the maximum lookahead distance
        offset = Vector3.ClampMagnitude(offset, maxLookAheadDistance);

        //smoothly adjust the camera offset
        currentLookAhead = Vector3.Lerp(currentLookAhead, offset, lookAheadSpeed * Time.deltaTime);

        //invert offset if player is facing left
        transposer.m_TrackedObjectOffset = new Vector3(
            currentLookAhead.x * ((player.LeftOrRight == CharacterController.Facing.left) ? -1 : 1),
            currentLookAhead.y,
            currentLookAhead.z);
    }
}
