using UnityEngine;

public class VoiceModule : IUpgrade
{
    /// <summary>
    /// What voices Atlas can speak in(or if they can't speak
    /// </summary>
    ///
    [SerializeField] private PlayerInfo playerInfo;
    public enum VoiceTypes
    {
        NONE,
        ATLAS,
        VOX
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
        playerInfo._currentVoice = VoiceTypes.VOX;
        TurnOn();
    }

    public override void TurnOn()
    {
        gameObject.SetActive(true);
    }
}
