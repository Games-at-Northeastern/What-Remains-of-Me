using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a move where the player is swinging from side to side whilst
/// connected to a wire.
/// </summary>
public class WireSwing : AMove
{
    private readonly float initRadius; // Radius from you to wire upon connection

    private bool disconnected; // To be true the frame the player is disconnected
    private float angularVelocity = 0;
    private float angularAccel;
    private float gravityVel; // Y Vel contributed by gravity during free fall
    private Vector2 vel;
    private bool inBounceMode; // Currently in the process of bouncing off a wall/ceiling?

    private bool dashInput; // Lets you dash while connected
    private bool damageInput;

    // Initializes a wire control move, deciding what angular vel to start with, setting the
    // wire's max distance, and initializing events.
    public WireSwing(float horizVel, float vertVel)
    {
        // Get starting angular vel
        Vector2 origPos = MI.transform.position;
        Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;

        // Set the wire length to be the current distance between the player and the outlet
        initRadius = Vector2.Distance(origPos, connectedOutletPos);
        WT.SetMaxWireLength(initRadius);

        float firstAngle = Mathf.Atan2(origPos.y - connectedOutletPos.y, origPos.x - connectedOutletPos.x);
        Vector2 newPos = origPos + (new Vector2(horizVel, vertVel) * Time.deltaTime);
        float secondAngle = Mathf.Atan2(newPos.y - connectedOutletPos.y, newPos.x - connectedOutletPos.x);
        float startingAngularVel = (secondAngle - firstAngle) / Time.deltaTime;
        angularVelocity = startingAngularVel;
        // Set up event responses
        WT.onDisconnect.AddListener(() => disconnected = true);
        PH.OnDamageTaken.AddListener(() => damageInput = true);
        CS.Player.Dash.performed += _ => dashInput = true;
    }

    public override void AdvanceTime()
    {
        // If the wire has just disconnected, don't modify anything (would cause errors to try and refer to null outlet)
        if (WT.ConnectedOutlet == null)
        {
            return;
        }
        // Get initial positions
        Vector2 origPos = MI.transform.position;
        Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;
        // Angle going from outlet to player
        float angle = Mathf.Atan2(origPos.y - connectedOutletPos.y, origPos.x - connectedOutletPos.x);
        float radius = Vector2.Distance(origPos, connectedOutletPos);
        // Check for bouncing against wall/ceiling
        if (MI.LeftWallDetector.isColliding() || MI.RightWallDetector.isColliding() || MI.CeilingDetector.isColliding())
        {
            if (!inBounceMode)
            {
                inBounceMode = true;
                angularVelocity = -angularVelocity * MS.WireSwingBounceDecayMultiplier;
            }
        }
        else
        {
            inBounceMode = false;
        }

        // Check for dash input
        if (dashInput)
        {
            float change = MS.WireSwingAngularVelOfDash * (MS.WireSwingReferenceWireLength / GetCurrentWireLength());
            change = change * -Mathf.Sin(angle);
            angularVelocity = Flipped() ? -change : change;
            dashInput = false;
        }
        // Get Angular Acceleration
        float inputPower = CS.Player.Move.ReadValue<float>() * Mathf.Clamp(Mathf.Abs(Mathf.Sin(angle)), 0, 1);

        // if the player is actively colliding against something, only consider the manual input to prevent infinite force buildup
        if (inBounceMode)
        {
            angularAccel = inputPower * MS.WireSwingManualAccelMultiplier;
        }
        else
        {
            angularAccel = (-Mathf.Cos(angle) * MS.WireSwingNaturalAccelMultiplier) + (inputPower * MS.WireSwingManualAccelMultiplier);
        }
        // Add decay if accel and vel are in different directions
        if (Mathf.Sign(angularAccel) != Mathf.Sign(angularVelocity))
        {
            angularAccel *= MS.WireSwingDecayMultiplier;
        }
        // Use the angular acceleration to change the angular vel, enact angular vel
        angularVelocity += angularAccel * Time.deltaTime * (MS.WireSwingReferenceWireLength / GetCurrentWireLength());

        // Clamp the angular velocity - again, to prevent infinite buildup or crazy speeds.
        angularVelocity = Mathf.Clamp(angularVelocity, -MS.WireSwingMaxAngularVelocity, MS.WireSwingMaxAngularVelocity);

        float newAngle = angle + (angularVelocity * Time.deltaTime);
        Vector2 newPos = connectedOutletPos + (radius * new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)));
        vel = (newPos - origPos) / Time.deltaTime;
        // Add some free-fall to the mix if above the lowest point possible right now
        if (initRadius - radius > 0.01f || Mathf.Sin(angle) > 0)
        {
            gravityVel -= MS.FallGravity * Time.deltaTime;
            vel += new Vector2(0, gravityVel);
        }
        else
        {
            gravityVel = 0;
        }
    }

    public override float XSpeed() => disconnected ? 0 : vel.x;

    // If the vel isn't 0 upon disconnect, weird bugs will happen
    public override float YSpeed() => disconnected ? 0 : vel.y;

    // If the vel isn't 0 upon disconnect, weird bugs will happen
    public override IMove GetNextMove()
    {
        if (damageInput)
        {
            return new Knockback();
        }
        if (MI.GroundDetector.isColliding())
        {
            WT.SetMaxWireLength(MS.WireGeneralMaxDistance);
            return new Run(vel.x);
        }
        if (disconnected)
        {
            WT.SetMaxWireLength(MS.WireGeneralMaxDistance);
            return new WireSwingRelease(vel.x, vel.y);
        }
        return this;
    }

    public override bool DisconnectByJumpOkay() => true;

    public override AnimationType GetAnimationState() => AnimationType.WIRE_SWING;
}
