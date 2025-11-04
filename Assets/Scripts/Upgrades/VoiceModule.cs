using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceModule : IUpgrade
{
    /// <summary>
    /// What voices Atlas can speak in(or if they can't speak
    /// </summary>
    ///
    [SerializeField] private PlayerInfo playerInfo;
    private ControlSchemes _controlSchemes;
    public PlayerController2D pc;
    private float countDownForSwap = 0f;
    private List<VoiceTypes> voiceList = new()
        {
            VoiceTypes.NONE,
            VoiceTypes.ATLAS,
            VoiceTypes.VOX
        };
    public enum VoiceTypes
    {
        NONE,
        ATLAS,
        VOX
    }

    public static string VoiceTypeString(VoiceTypes type) => type switch
    {
        VoiceTypes.NONE => "VoiceNone",
        VoiceTypes.ATLAS => "VoiceAtlas",
        VoiceTypes.VOX => "VoiceVox",
        _ => throw new System.ArgumentException("VoiceType not found"),
    };

    private void Awake()
    {
        _controlSchemes = new ControlSchemes();
        _controlSchemes.Enable();
        _controlSchemes.Player.SwitchVoiceModule.started += HandleVoiceModuleKeyboard;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override UpgradeType GetUpgradeType() => UpgradeType.VOICEMODULE;
    public override void Aquire()
    {
        playerInfo.AddVoices(new List<VoiceTypes> { VoiceTypes.ATLAS, VoiceTypes.VOX });
        TurnOn();
    }

    public override void TurnOn()
    {
        gameObject.SetActive(true);
    }


    /// <summary>
    /// Function that is passed to the control scheme to handle cancelling a throw when the
    /// keyboard/mouse button for this action is released.
    /// </summary>
    private void HandleVoiceModuleKeyboard(InputAction.CallbackContext ctx)
    {
    }
    private void OnDestroy() => _controlSchemes.Player.SwitchVoiceModule.started -= HandleVoiceModuleKeyboard;
}
