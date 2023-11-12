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
        private ICharacterController character;
        private float horizontalInput;
        private float WireSwingManualAccelMultiplier;
        private float WireSwingBounceDecayMultiplier;
        private float WireSwingReferenceWireLength;
        private float WireSwingMaxAngularVelocity;
        private float WireSwingNaturalAccelMultiplier;
        private float fallGravity;
        private float wireGeneralMaxDistance;
        private MovementInfo MI;

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
        /// <param name="WireSwingManualAccelMultiplier">I assume the acceleration for the player kicking back and forth</param>
        /// <param name="wireGeneralMaxDistance"></param>
        /// <param name="MI"></param>
        public Swing(WireThrower WT, ICharacterController character, float fallGravity,float WireSwingNaturalAccelMultiplier, float WireSwingMaxAngularVelocity, float WireSwingDecayMultiplier, float WireSwingBounceDecayMultiplier, float WireSwingReferenceWireLength, float WireSwingManualAccelMultiplier, float wireGeneralMaxDistance, MovementInfo MI)
        {
            this.WireSwingManualAccelMultiplier = WireSwingManualAccelMultiplier;
            this.wireGeneralMaxDistance = wireGeneralMaxDistance;
            this.WireSwingBounceDecayMultiplier = WireSwingBounceDecayMultiplier;
            this.WireSwingMaxAngularVelocity = WireSwingMaxAngularVelocity;
            this.WireSwingNaturalAccelMultiplier = WireSwingNaturalAccelMultiplier;
            this.fallGravity = fallGravity;
            this.WireSwingReferenceWireLength = WireSwingReferenceWireLength;
            this.WT = WT;
            this.WireSwingDecayMultiplier = WireSwingDecayMultiplier;
            this.character = character;
            this.MI = MI;
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
            // If the wire has just disconnected, don't modify anything (would cause errors to try and refer to null outlet)
            if (WT.ConnectedOutlet == null)
            {
                return;
            }
            // Get initial positions
            Vector2 origPos = character.position;
            Vector2 connectedOutletPos = WT.ConnectedOutlet.transform.position;
            // Angle going from outlet to player
            float angle = Mathf.Atan2(origPos.y - connectedOutletPos.y, origPos.x - connectedOutletPos.x);
            float radius = Vector2.Distance(origPos, connectedOutletPos);
            // Check for bouncing against wall/ceiling
            if (character.TouchingLeftWall() || character.TouchingRightWall() || character.TouchingCeiling())
            {
                if (!inBounceMode)
                {
                    inBounceMode = true;
                    angularVelocity = -angularVelocity * WireSwingBounceDecayMultiplier;
                }
            }
            else
            {
                inBounceMode = false;
            }

            // Check for dash input
            /*
            // COMMENTING OUT THE ABILITY TO DASH WHILE WIRE IS ATTACHED FOR RIGHT NOW (SUMMER 2023) BECAUSE CHAPTER 1 DOESN'T NEED IT
            if (dashInput)
            {
                float change = MS.WireSwingAngularVelOfDash * (MS.WireSwingReferenceWireLength / GetCurrentWireLength());
                change = change * -Mathf.Sin(angle);
                angularVelocity = Flipped() ? -change : change;
                dashInput = false;
            }*/
            // Get Angular Acceleration
            float inputPower = horizontalInput * Mathf.Clamp(Mathf.Abs(Mathf.Sin(angle)), 0, 1);

            // if the player is actively colliding against something, only consider the manual input to prevent infinite force buildup
            if (inBounceMode)
            {
                angularAccel = inputPower * WireSwingManualAccelMultiplier;
            }
            else
            {
                angularAccel = (-Mathf.Cos(angle) * WireSwingNaturalAccelMultiplier) + (inputPower * WireSwingManualAccelMultiplier);
            }
            // Add decay if accel and vel are in different directions
            if (Mathf.Sign(angularAccel) != Mathf.Sign(angularVelocity))
            {
                angularAccel *= WireSwingDecayMultiplier;
            }
            // Use the angular acceleration to change the angular vel, enact angular vel
            angularVelocity += angularAccel * Time.deltaTime * (WireSwingReferenceWireLength / GetCurrentWireLength());

            // Clamp the angular velocity - again, to prevent infinite buildup or crazy speeds.
            angularVelocity = Mathf.Clamp(angularVelocity, -WireSwingMaxAngularVelocity, WireSwingMaxAngularVelocity);

            float newAngle = angle + (angularVelocity * Time.deltaTime);

            // To accommodate the downwards movement of moving grapple points
            radius = Mathf.Lerp(radius, initRadius, Time.deltaTime * 10f);

            Vector2 newPos = connectedOutletPos + (radius * new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle)));
            character.SetSpeed((newPos - origPos) / Time.deltaTime);
            // Add some free-fall to the mix if above the lowest point possible right now
            //if (initRadius - radius > 0.01f || Mathf.Sin(angle) > 0)
            if (Mathf.Sin(angle) > 0)
            {
                gravityVel -= fallGravity * Time.deltaTime;
                character.SetSpeed(character.Speed + new Vector2(0, gravityVel));
            }
            else
            {
                gravityVel = 0;
            }
        }
        /// <summary>
        /// Get the length of the wire connecting to an outlet. You should be
        /// connected to an outlet if you're calling this function.
        /// </summary>
        protected float GetCurrentWireLength() => Vector2.Distance(character.position, WT.ConnectedOutlet.transform.position);
        public void CancelMove()
        {
            WT.SetMaxWireLength(wireGeneralMaxDistance);
            // If the vel isn't 0 upon disconnect, weird bugs will happen (line in old cold may need to keep this in mind)

        }


        public AnimationType GetAnimationState() => AnimationType.WIRE_SWING;

        public bool IsMoveComplete() => false; // Decided that move should maybe never be considered be complete cause its really for the controller to decide when to disconnect 

    }
}
