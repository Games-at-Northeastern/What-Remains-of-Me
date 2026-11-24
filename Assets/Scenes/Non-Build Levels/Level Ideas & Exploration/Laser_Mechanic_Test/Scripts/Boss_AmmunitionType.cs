using System;
using UnityEngine;

/// <summary>
///     Ammunition module: decides what happens when a laser hits the player.
///     There are 2 ammunition types:
///     1. Death: kills the player (same as the death lasers).
///     2. Virus: turns the player's clean energy into virus energy. If the player has no energy, nothing happens.
///     It also decides how the shots look (beam material, colors, and bullet sprite).
///     Use together with Boss_LaserType and Boss_SentryType.
///     If this component is turned off, the shots do nothing (useful for testing movement only).
/// </summary>
public class Boss_AmmunitionType : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Death: kills the player.\nVirus: turns the player's clean energy into virus energy.")]
    private AmmunitionType ammunitionType;

    [Header("Death")]
    [SerializeField]
    [Tooltip("Kills and respawns the player. If empty, uses the Death Teleport inside the turret.")]
    private SpikeTeleport deathTeleporter;
    [SerializeField]
    [Range(1.5f, 10f)]
    [Tooltip("The amount of time before the laser could hit the player again!")]
    private float lockoutTime = 1.5f;

    [Header("Virus")]
    [SerializeField]
    [Tooltip("Virus added by one bullet or one sniper shot.")]
    private float virusPerHit = 10f;
    [SerializeField]
    [Tooltip("Virus added per second while the player is inside a beam.")]
    private float virusPerSecond = 20f;

    [Header("Death Look")]
    [SerializeField]
    [Tooltip("Material for beams, aim lines and bullets (use the death laser material).")]
    private Material deathLineMaterial;
    [SerializeField]
    [Tooltip("Color of beams and aim lines.")]
    private Color deathLineColor = Color.white;
    [SerializeField]
    [Tooltip("Color of laser bullets.")]
    private Color deathBulletColor = new Color(1f, 0.15f, 0.15f, 1f);

    [Header("Virus Look")]
    [SerializeField]
    [Tooltip("Material for beams, aim lines and bullets (use the virus beam material).")]
    private Material virusLineMaterial;
    [SerializeField]
    [Tooltip("Color of beams and aim lines.")]
    private Color virusLineColor = Color.white;
    [SerializeField]
    [Tooltip("Color of laser bullets.")]
    private Color virusBulletColor = new Color(0.85f, 0.45f, 1f, 1f);

    [Header("Sound")]
    [SerializeField]
    [Tooltip("Plays when a Death laser kills the player.")]
    private AudioClip deathClip;
    [SerializeField]
    [Tooltip("Plays when a Virus laser adds virus to the player.")]
    private AudioClip virusHitClip;
    [SerializeField]
    [Tooltip("While the player stays in a virus beam, the virus sound repeats this often (seconds).")]
    private float virusSoundInterval = 0.5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float volume = 0.8f;

    private bool lockout; // Death timeout for the player
    private float nextVirusSoundTime;

    public bool IsVirus => ammunitionType == AmmunitionType.Virus;

    public Material LineMaterial => IsVirus ? virusLineMaterial : deathLineMaterial;

    public Color LineColor => IsVirus ? virusLineColor : deathLineColor;

    public Color BulletColor => IsVirus ? virusBulletColor : deathBulletColor;

    private void Awake()
    {
        if (deathTeleporter == null)
        {
            deathTeleporter = GetComponentInChildren<SpikeTeleport>(true);
        }
    }

    /// <summary>
    ///     A single hit (bullet or sniper shot).
    /// </summary>
    /// <param name="player"> The player that was hit </param>
    public void HitOnce(Transform player)
    {
        if (!enabled || player == null)
        {
            return;
        }

        if (IsVirus)
        {
            AddVirus(virusPerHit);
        }
        else
        {
            KillPlayer(player);
        }
    }

    /// <summary>
    ///     A hit that lasts while the player stays in a beam. Call this every frame the beam touches the player.
    /// </summary>
    /// <param name="player"> The player that was hit </param>
    /// <param name="deltaTime"> Time since the last frame </param>
    public void HitOverTime(Transform player, float deltaTime)
    {
        if (!enabled || player == null)
        {
            return;
        }

        if (IsVirus)
        {
            AddVirus(virusPerSecond * deltaTime);
        }
        else
        {
            KillPlayer(player);
        }
    }

    /// <summary>
    ///     Kills the player, the same way the death lasers do.
    /// </summary>
    private void KillPlayer(Transform player)
    {
        if (lockout || deathTeleporter == null || PlayerRef.PlayerManager.GodMode)
        {
            return;
        }

        // Teleport Atlas
        StartCoroutine(deathTeleporter.PerformDeath(player.gameObject));
        // Play death sound
        PlayClip(deathClip);
        // Start lockout time
        lockout = true;
        Invoke(nameof(LockoutCooldownInvocation), lockoutTime);
    }

    /// <summary>
    ///     Turns clean energy into virus energy. Battery is the player's total energy, and Virus is the infected part of it,
    ///     so virus can never go above the total. With no energy, nothing happens.
    /// </summary>
    /// <param name="amount"> How much clean energy to turn into virus </param>
    private void AddVirus(float amount)
    {
        EnergyManager energyManager = PlayerRef.PlayerManager.EnergyManager;
        if (energyManager == null || energyManager.Battery <= 0.01f)
        {
            return; // Virus can't come from no energy
        }

        float limit = Mathf.Min(energyManager.Battery, energyManager.MaxVirus);
        energyManager.Virus = Mathf.Min(energyManager.Virus + amount, limit);

        // Don't repeat the sound every frame while the player is in a beam
        if (Time.time >= nextVirusSoundTime)
        {
            PlayClip(virusHitClip);
            nextVirusSoundTime = Time.time + virusSoundInterval;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        // Play at the camera so the sound is always heard in 2D
        Vector3 position = Camera.main != null ? Camera.main.transform.position : transform.position;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    private void LockoutCooldownInvocation() => lockout = false;

    [Serializable]
    private enum AmmunitionType
    {
        Death, Virus
    }
}
