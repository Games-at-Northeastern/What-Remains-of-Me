using System.Reflection;
using UnityEngine;

/// <summary>
///     Test only: makes Atlas immune to lasers in this scene.
///     Check or uncheck "Immortal" in the Inspector (it also works during Play Mode).
///     It turns on the player's existing God Mode flag, which the death lasers and the Boss_ laser modules already check.
///     Unlike the God Mode button, it does not change Atlas's movement (no debug stats, no infinite jumps).
///     It only affects the scene this object is in, and turns itself off when the object is disabled or the scene ends.
///     Virus lasers still add virus, so virus mechanics can still be tested.
/// </summary>
public class Test_immortal_Atlas : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Check to make Atlas immune to lasers in this scene.")]
    private bool immortal;

    // PlayerManager.GodMode has a private setter, so set its backing field directly (test only)
    private static readonly FieldInfo GodModeField =
        typeof(PlayerManager).GetField("<GodMode>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    private bool applied; // What was last sent to the player

    private void Start()
    {
        if (GodModeField == null)
        {
            Debug.LogWarning("Test_immortal_Atlas: could not find PlayerManager.GodMode. Did PlayerManager change?");
        }

        Apply(immortal);
    }

    /// <summary>
    ///     Applies the checkbox whenever it changes in the Inspector.
    /// </summary>
    private void Update()
    {
        if (immortal != applied)
        {
            Apply(immortal);
        }
    }

    private void OnDisable() => Apply(false);

    /// <summary>
    ///     Turns the player's God Mode flag on or off.
    /// </summary>
    /// <param name="on"> True to make Atlas immune </param>
    private void Apply(bool on)
    {
        PlayerManager playerManager = PlayerRef.PlayerManager;
        if (playerManager == null || GodModeField == null)
        {
            return;
        }

        GodModeField.SetValue(playerManager, on);
        applied = on;
        Debug.Log("Test_immortal_Atlas: Atlas is " + (on ? "immortal" : "mortal") + " in this scene.");
    }
}
