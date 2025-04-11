
using UnityEngine;
using Cinemachine;
using PlayerController;

public class BiasCameraForward : MonoBehaviour {
    private PlayerController2D player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float screenBias = 0.1f;
    [SerializeField] private float smoothingSpeed = 5f;

    private CinemachineFramingTransposer transposer;
    private float targetScreenX;
    private float currentScreenX;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentScreenX = 0.5f;
        transposer.m_ScreenX = currentScreenX;
    }

    private void Update() {
        targetScreenX = player.LeftOrRight == CharacterController.Facing.left
            ? 0.5f - screenBias
            : 0.5f + screenBias;

        currentScreenX = Mathf.Lerp(currentScreenX, targetScreenX, smoothingSpeed * Time.deltaTime);
        transposer.m_ScreenX = currentScreenX;
    }
}
