
using UnityEngine;

using PlayerController;

public class BiasCameraForward : MonoBehaviour {
    private PlayerController2D player;
    [SerializeField] private Unity.Cinemachine.CinemachineCamera virtualCamera;
    [SerializeField] private float screenBias = 0.1f;
    [SerializeField] private float smoothingSpeed = 5f;

    private Unity.Cinemachine.CinemachinePositionComposer composer;
    private float targetScreenX;
    private float currentScreenX;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();
        composer = virtualCamera.GetCinemachineComponent(
            Unity.Cinemachine.CinemachineCore.Stage.Body
        ) as Unity.Cinemachine.CinemachinePositionComposer;
        currentScreenX = 0.5f;
        var composition = composer.Composition;
        composition.ScreenPosition.x = currentScreenX;
        composer.Composition = composition;
    }

    private void Update() {
        targetScreenX = player.LeftOrRight == CharacterController.Facing.left
            ? 0.5f - screenBias
            : 0.5f + screenBias;

        currentScreenX = Mathf.Lerp(currentScreenX, targetScreenX, smoothingSpeed * Time.deltaTime);
        var composition = composer.Composition;
        composition.ScreenPosition.x = currentScreenX;
        composer.Composition = composition;
    }
}
