using TMPro;
using UnityEngine;

public class ToggleMouseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private GameObject playerCamera;

    private bool on = false;

    private BiasCameraForward playerBiasCameraForward;
    private LookAheadCamera playerLookAheadCamera;

    private void Awake() {
        playerCamera = GameObject.FindGameObjectWithTag("PlayerCamera");
        playerBiasCameraForward = playerCamera.GetComponent<BiasCameraForward>();
        playerLookAheadCamera = playerCamera.GetComponent<LookAheadCamera>();
    }

    public void OnClick() {
        if (on) {
            text.text = "Toggle Mouse Camera: Off";
        } else {
            text.text = "Toggle Mouse Camera: On";
        }

        on = !on;
        playerLookAheadCamera.enabled = !playerLookAheadCamera.enabled;
        playerBiasCameraForward.enabled = !playerBiasCameraForward.enabled;
    }
}
