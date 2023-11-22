using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An abstract class for scripts that toggle gameobjects under certain conditions.
/// This class provides the basic implementation for the event itself.
///
/// Click here for more info as to why this extends AControllable: https://discord.com/channels/847442130300567582/895293142750859294/1169710612322975875
/// </summary>
public abstract class ATriggerToggle : AControllable
{
    [Space(20)]
    [Header("ATriggerToggle Fields")]
    [SerializeField]
    protected bool _enabledOnStart = true;
    [SerializeField]
    private UnityEvent<bool> ToggleableElements;

    protected virtual void Awake() => FireEvent(_enabledOnStart);

    protected virtual void FireEvent(bool state) => ToggleableElements.Invoke(state);

}
