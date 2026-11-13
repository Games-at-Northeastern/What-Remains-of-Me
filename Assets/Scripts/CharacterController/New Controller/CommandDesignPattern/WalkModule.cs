using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WalkModule : MonoBehaviour, IPlayerModule
{
    public RaycastPlayerController Controller { get; set; }
    public float HealthPercentile { get; private set; } = 1f;

    [SerializeField] float maxSpeed = 0.15f; // units per physics step, same units as MoveDisplacement

    // How fast speed changes, as a fraction of maxSpeed per second.
    // 1 = 0 → max in 1 second, 2 = in 0.5 seconds, etc.
    [FormerlySerializedAs("accelTimeMutliplier")]
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
            rate = decelTimeMultiplier;                       // no input: slow down
        else if (x != 0f && Mathf.Sign(input) != Mathf.Sign(x))
            rate = redirectTimeMultiplier;                    // pressing against current motion
        else if (Mathf.Abs(x) > maxSpeed)
            rate = decelTimeMultiplier;                       // faster than walking (e.g. launch): ease down
        else
            rate = accelTimeMultiplier;                       // speeding up

        Controller.MoveDisplacement.x = Mathf.MoveTowards(x, target, maxSpeed * rate * Time.deltaTime);
    }
}