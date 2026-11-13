using UnityEngine;
using PlayerController;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
public class JumpModule : MonoBehaviour, IPlayerModule
{
    public RaycastPlayerController Controller { get; set; }
    public float HealthPercentile { get; private set; } = 1f;

    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpHeight = 28f;

    private float coyoteTimer = 0;
    private float jumpForce;


    void OnEnable()
    {

        Controller = transform.GetComponent<RaycastPlayerController>();
        //jumpForce = Mathf.Sqrt(2 * jumpHeight * Controller.GetGravity());

        float g = Controller.GetGravity();
        jumpForce = (g + Mathf.Sqrt(g * g + 8f * g * jumpHeight)) / 2f;
    }


    public void Damage(float subtractPercent)
    {
        HealthPercentile -= subtractPercent;
    }


    void Update()
    {
        if (Controller.Grounded())
        {
            coyoteTimer = 0;
        }
        else
        {
            coyoteTimer += Time.deltaTime;
        }

        var kb = Keyboard.current;
        if (kb == null)
            return;

        if (kb.spaceKey.wasPressedThisFrame && coyoteTimer <= coyoteTime)
        {
            Controller.MoveDisplacement.y = jumpForce;
        }
    }
}
