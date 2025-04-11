using PlayerController;
using TMPro;
using UnityEngine;

public class GodModeButton : MonoBehaviour
{
    [SerializeField] private PlayerSettings defaultSettings;
    [SerializeField] private PlayerSettings godModeSettings;

    private PlayerController2D player;

    [SerializeField] private TMP_Text text;

    private bool on = false;

    private void Awake() => player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();

    public void OnClick() {
        if (!player) {
            return;
        }
            

        if (on) {
            player.stats_ = defaultSettings;
            text.text = "God Mode: Off";
            on = false;
        } else {
            player.stats_ = godModeSettings;
            text.text = "God Mode: On";
            on = true;
        }

        player.SetupMoves();
    }
}
