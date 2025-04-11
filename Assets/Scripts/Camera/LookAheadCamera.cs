using UnityEngine;
using Cinemachine;
using PlayerController;

public class LookAheadCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    private PlayerController2D player;
    [SerializeField] private float maxLookAheadDistance = 3.5f;
    [SerializeField] private float lookAheadSpeed = 2f;

    private CinemachineFramingTransposer transposer;
    private Vector3 currentLookAhead;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        transposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            currentLookAhead = transposer.m_TrackedObjectOffset;
        }
    }

    private void LateUpdate()
    {
        if (transposer == null || player == null || mainCamera == null)
            return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector3 screenCenter = GetScreenCenter();
        Vector3 offset = ClampOffset(mouseWorldPos, screenCenter);

        SmoothLookAhead(offset);
        ApplyOffset();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

    private Vector3 GetScreenCenter()
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, mainCamera.nearClipPlane));
    }

    private Vector3 ClampOffset(Vector3 mousePos, Vector3 screenCenter)
    {
        Vector3 offset = mousePos - screenCenter;
        return Vector3.ClampMagnitude(offset, maxLookAheadDistance);
    }

    private void SmoothLookAhead(Vector3 targetOffset)
    {
        currentLookAhead = Vector3.Lerp(currentLookAhead, targetOffset, lookAheadSpeed * Time.deltaTime);
    }

    private void ApplyOffset()
    {
        float directionMultiplier = player.LeftOrRight == CharacterController.Facing.left ? -1f : 1f;
        transposer.m_TrackedObjectOffset = new Vector3(
            currentLookAhead.x * directionMultiplier,
            currentLookAhead.y,
            currentLookAhead.z
        );
    }
}
