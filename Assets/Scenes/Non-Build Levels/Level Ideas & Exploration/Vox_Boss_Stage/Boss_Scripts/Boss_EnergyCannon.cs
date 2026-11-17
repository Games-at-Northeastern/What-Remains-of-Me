using UnityEngine;
using UnityEngine.InputSystem;

public class Boss_EnergyCannon : AControllable
{
    [Header("Aiming")]
    [SerializeField]
    [Tooltip("The part that rotates.")]
    private Transform barrel;

    [SerializeField]
    [Tooltip("Outlets that let Atlas aim and fire this cannon while plugged in.")]
    private Outlet[] aimOutlets;

    [SerializeField]
    [Tooltip("How fast the barrel turns toward the wire direction (degrees per second).")]
    private float aimTurnSpeed = 360f;

    [SerializeField]
    [Tooltip("Limit how far the barrel can turn.")]
    private bool clampAim;

    [SerializeField]
    [Tooltip("Aim limits in degrees, measured from straight up. Neg = left, Pos = right.")]
    private float minAimAngle = -120f;

    [SerializeField]
    private float maxAimAngle = 120f;

    [Header("Firing")]
    [SerializeField]
    [Tooltip("Key used to fire while plugged in.")]
    private Key fireKey = Key.G;

    [SerializeField]
    private float energyPerShot = 20f;

    [SerializeField]
    private float fireCooldown = 0.5f;

    [SerializeField]
    [Tooltip("How far in front of the barrel pivot the projectile appears.")]
    private float muzzleDistance = 1.2f;

    [SerializeField]
    private Boss_CannonProjectile.Settings projectile = new Boss_CannonProjectile.Settings
    {
        speed = 14f,
        range = 30f,
        radius = 0.35f,
        color = new Color(0.4f, 0.9f, 1f)
    };

    [SerializeField]
    [Tooltip("Color used when the shot was made with mostly virus energy.")]
    private Color virusShotColor = new Color(0.55f, 1f, 0.25f);

    [SerializeField]
    [Tooltip("Optional SoundController sound name played when firing.")]
    private string fireSoundName = "";

    private float cooldownTimer;
    private Transform player;

    private void Awake()
    {
        if (barrel == null)
        {
            barrel = transform;
        }

        if (maxCharge <= 0f)
        {
            maxCharge = 60f;
        }
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        Outlet connected = GetConnectedOutlet();
        if (connected == null)
        {
            return;
        }

        AimAlongWire(connected);

        if (Keyboard.current != null && Keyboard.current[fireKey].wasPressedThisFrame)
        {
            TryFire();
        }
    }

    private Outlet GetConnectedOutlet()
    {
        if (aimOutlets == null)
        {
            return null;
        }

        foreach (Outlet outlet in aimOutlets)
        {
            if (outlet != null && outlet.isConnected)
            {
                return outlet;
            }
        }

        return null;
    }

    private Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            player = found != null ? found.transform : null;
        }

        return player;
    }

    /// <summary>
    ///     Turns the barrel so it points along the wire, from Atlas through the outlet.
    /// </summary>
    private void AimAlongWire(Outlet outlet)
    {
        Transform atlas = GetPlayer();
        if (atlas == null)
        {
            return;
        }

        Vector2 wire = (Vector2)outlet.transform.position - (Vector2)atlas.position;
        if (wire.sqrMagnitude < 0.01f)
        {
            return;
        }

        if (clampAim)
        {
            float angleFromUp = Mathf.Atan2(wire.x, wire.y) * Mathf.Rad2Deg;
            angleFromUp = Mathf.Clamp(angleFromUp, minAimAngle, maxAimAngle);
            float rad = angleFromUp * Mathf.Deg2Rad;
            wire = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        }

        Vector2 currentDirection = FireDirection;
        float delta = Vector2.SignedAngle(currentDirection, wire);
        float maxStep = aimTurnSpeed * Time.deltaTime;
        barrel.Rotate(0f, 0f, Mathf.Clamp(delta, -maxStep, maxStep), Space.World);
    }

    private Vector2 FireDirection => -(Vector2)barrel.up;

    public void TryFire()
    {
        if (cooldownTimer > 0f)
        {
            return;
        }

        if (totalEnergy < energyPerShot)
        {
            Debug.Log($"{name}: not enough energy to fire ({totalEnergy:0}/{energyPerShot:0}).");
            return;
        }

        float shotVirusRatio = GetVirusPercent() ?? 0f;
        LeakEnergy(energyPerShot);
        EnergyChange(-energyPerShot);
        cooldownTimer = fireCooldown;

        Vector2 direction = FireDirection;
        Vector2 muzzle = (Vector2)barrel.position + direction * muzzleDistance;
        Color color = Color.Lerp(projectile.color, virusShotColor, shotVirusRatio);

        Boss_CannonProjectile.Spawn(muzzle, direction, projectile, false, true, shotVirusRatio, null, color);

        if (!string.IsNullOrEmpty(fireSoundName) && SoundController.instance != null)
        {
            SoundController.instance.PlaySound(fireSoundName);
        }
    }
}
