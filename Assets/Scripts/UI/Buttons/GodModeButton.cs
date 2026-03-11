using PlayerController;
using TMPro;
using UnityEngine;
public class GodModeButton : MonoBehaviour
{
    [SerializeField] private PlayerSettings defaultSettings;
    [SerializeField] private PlayerSettings godModeSettings;

    [SerializeField] private TMP_Text text;

    private bool on;

    private PlayerController2D player;

    private void Awake() => player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController2D>();

    public void OnClick()
    {
        if (!player) {
            return;
        }


        if (on) {
            PlayerRef.PlayerManager.SetGodModeStats(this, false);
            text.text = "God Mode: Off";
            on = false;
        } else {
            PlayerRef.PlayerManager.SetGodModeStats(this, true);
            text.text = "God Mode: On";
            on = true;
        }

        player.SetupMoves();
    }
}
