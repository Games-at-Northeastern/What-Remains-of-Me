using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WalkModule : MonoBehaviour, IPlayerModule
{
    public RaycastPlayerController Controller { get; set; }
    public float HealthPercentile { get; private set; } = 1f;
    [SerializeField] float maxSpeed = 0.2f; // units per physics step, same units as MoveDisplacement
    [Header("Directional Acceleration Knobs")]
    // How fast speed changes, as a fraction of maxSpeed per second.
    //used to use an animation curve and intended to make acceleration more customizeable. overconvoluted, now acceleration is linear like the old PlayerController2D
    [SerializeField] float accelTimeMultiplier = 1f;
    [SerializeField] float decelTimeMultiplier = 2f;
    [SerializeField] float redirectTimeMultiplier = 2f;

    void OnEnable()
    {
        Controller = GetComponent<RaycastPlayerController>();
    }

    public void Damage(float amount)
    {
        HealthPercentile = Mathf.Clamp01(HealthPercentile - amount);
        if (HealthPercentile <= 0f)
            enabled = false;
    }

    void Update()
    {
        if (Controller.OnRope)
            return;

        var kb = Keyboard.current;
        if (kb == null)
            return;

        float input = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
            input -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
            input += 1f;

        // Start from whatever the current velocity is, including a grapple launch
        float x = Controller.MoveDisplacement.x;
        float target = input * maxSpeed;

        float rate;
        if (input == 0f)
            rate = decelTimeMultiplier;
        else if (x != 0f && Mathf.Sign(input) != Mathf.Sign(x))
            rate = redirectTimeMultiplier;
        else if (Mathf.Abs(x) > maxSpeed)
            rate = decelTimeMultiplier;
        else
            rate = accelTimeMultiplier;

        Controller.MoveDisplacement.x = Mathf.MoveTowards(x, target, maxSpeed * rate * Time.deltaTime);
    }
}