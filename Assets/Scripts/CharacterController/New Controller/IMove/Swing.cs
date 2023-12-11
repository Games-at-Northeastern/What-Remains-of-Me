using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CharacterController
{
    public class Swing : IMove
    {
        private float initRadius;// Radius from you to wire upon connection
        private float angularVelocity = 0;
        private float angularAccel;
        private float gravityVel; // Y Vel contributed by gravity during free fall
        private bool inBounceMode; // Currently in the process of bouncing off a wall/ceiling?
        private WireThrower WT;
        private float WireSwingDecayMultiplier;
        private CharacterController2D character;
        private float horizontalInput;
        private float PlayerSwayAcceleration;
        private float SwingBounceDecayMultiplier;
        private float MaxAngularVelocity;
        private float WireSwingNaturalAccelMultiplier;
        private float fallGravity;
        private float maxWireLength;

        // Initializes a wire control move, deciding what angular vel to start with, setting the
        // wire's max distance, and initializing events.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WT">The wireThrower</param>
        /// <param name="character"> the character swing from a wire</param>
        /// <param name="fallGravity"> the gravity exerted on the character while swinging</param>
        /// <param name="WireSwingNaturalAccelMultiplier"> A natural Acceleration force on the swing(IAssume if there is no input from the player)</param>
        /// <param name="WireSwingMaxAngularVelocity"> max angular velocity</param>
        /// <param name="WireSwingDecayMultiplier"> deceleration on the wire</param>
        /// <param name="WireSwingBounceDecayMultiplier"> energy loss upon bouncing on a wall</param>
        /// <param name="WireSwingReferenceWireLength"></param>
        /// <param name="PlayerSwayAccel">I assume the acceleration for the player kicking back and forth</param>
        /// <param name="wireGeneralMaxDistance"></param>
        public Swing(WireThrower WT, CharacterController2D character, float fallGravity,float WireSwingNaturalAccelMultiplier, float WireSwingMaxAngularVelocity, float WireSwingDecayMultiplier, float WireSwingBounceDecayMultiplier, float PlayerSwayAccel, float maxWireLength)
        {
            this.PlayerSwayAcceleration = PlayerSwayAccel;
            this.maxWireLength = maxWireLength;
            this.SwingBounceDecayMultiplier = WireSwingBounceDecayMultiplier;
            this.MaxAngularVelocity = WireSwingMaxAngularVelocity;
            this.WireSwingNaturalAccelMultiplier = WireSwingNaturalAccelMultiplier;
            this.fallGravity = fallGravity;
            this.WT = WT;
            this.WireSwingDecayMultiplier = WireSwingDecayMultiplier;
            this.character = character;
        }
        public void UpdateInput(float horizontalInput)
        {
            this.horizontalInput = horizontalInput;
        }
        public void StartMove()
        {
            Vector2 origPos = character.position;
            Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;

            initRadius = GetCurrentWireLength();
            WT.SetMaxWireLength(initRadius);

            float firstAngle = Mathf.Atan2(origPos.y - connectedOutletPos.y, origPos.x - connectedOutletPos.x);
            Vector2 newPos = origPos + (new Vector2(character.Velocity.x, character.Velocity.y) * Time.deltaTime);
            float secondAngle = Mathf.Atan2(newPos.y - connectedOutletPos.y, newPos.x - connectedOutletPos.x);
            float startingAngularVel = (secondAngle - firstAngle) / Time.deltaTime;
            angularVelocity = startingAngularVel;
        }
        public void ContinueMove()
        {
            string debugString = "Swing: ";
            // Get initial positions
            Vector2 origPos = character.position;
            Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;
            // Angle going from outlet to player
            float angle = Mathf.Atan2(origPos.y - connectedOutletPos.y, origPos.x - connectedOutletPos.x);
            float radius = Vector2.Distance(origPos, connectedOutletPos);
            debugString += angularVelocity + " ";
            // Check for bouncing against wall/ceiling
            if (character.TouchingLeftWall() || character.TouchingRightWall() || character.TouchingCeiling())
            {
                if (!inBounceMode)
                {
                    debugString += "BounceMode";
                    inBounceMode = true;
                    angularVelocity = -angularVelocity * SwingBounceDecayMultiplier;
                    Debug.Log("SBDM" + SwingBounceDecayMultiplier);
                    debugString += " " + angularVelocity;
                }
            }
            else
            {
                inBounceMode = false;
            }

            float inputPower = horizontalInput * Mathf.Clamp(Mathf.Abs(Mathf.Sin(angle)), 0, 1);

            // if the player is actively colliding against something, only consider the manual input to prevent infinite force buildup
            if (inBounceMode)
            {
                angularAccel = inputPower * PlayerSwayAcceleration;
            }
            else
            {
                angularAccel = (-Mathf.Cos(angle) * WireSwingNaturalAccelMultiplier) + (inputPower * PlayerSwayAcceleration);
            }
            // Add decay if accel and vel are in different directions
            if (Mathf.Sign(angularAccel) != Mathf.Sign(angularVelocity))
            {
                angularAccel *= WireSwingDecayMultiplier;
            }
            // Use the angular acceleration to change the angular vel, enact angular vel
            angularVelocity += angularAccel * Time.deltaTime;
            debugString += " " + angularVelocity;

            // Clamp the angular velocity - again, to prevent infinite buildup or crazy speeds.
            angularVelocity = Mathf.Clamp(angularVelocity, -MaxAngularVelocity, MaxAngularVelocity);

            float newAngle = angle + (angularVelocity * Time.deltaTime);

            // To accommodate the downwards movement of moving grapple points
            radius = Mathf.Lerp(radius, initRadius, Time.deltaTime * 10f);

            Vector2 newPos = connectedOutletPos + (radius * new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)));
            character.Speed = (newPos - origPos) / Time.deltaTime;
            // Add some free-fall to the mix if above the lowest point possible right now
            if (Mathf.Sin(angle) > 0)
            {
                gravityVel = fallGravity * Time.deltaTime;
                character.Speed += new Vector2(0, fallGravity * Time.deltaTime);
            }
            else
            {
                gravityVel = 0;
            }
            //Debug.Log(debugString);
        }
        /// <summary>
        /// Get the length of the wire connecting to an outlet. You should be
        /// connected to an outlet if you're calling this function.
        /// </summary>
        protected float GetCurrentWireLength() => Vector2.Distance(character.position, WT.ConnectedOutlet.transform.position);
        public void CancelMove()
        {
            WT.SetMaxWireLength(maxWireLength);
            // If the vel isn't 0 upon disconnect, weird bugs will happen (line in old cold may need to keep this in mind)

        }


        public AnimationType GetAnimationState() => AnimationType.WIRE_SWING;

        public bool IsMoveComplete() => false; // Decided that move should maybe never be considered be complete cause its really for the controller to decide when to disconnect 

    }
}
