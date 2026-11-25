using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    /// <summary>
    /// Player controller with the reworked movement physics.
    /// Adds faster turning, separate air control, momentum that carries over in the air, a height-based jump with
    /// heavier falling gravity and a wire swing that only limits the player's velocity, so collisions always win over
    /// the wire and releasing it keeps exactly the speed the player had.
    /// Run and air speed, ground acceleration, jump height, coyote time, jump buffer, fall speed limit and wire length
    /// are read from the active PlayerSettings every physics step, so god mode and upgrades still apply.
    /// </summary>
    public class PlayerPhysicsController2D : PlayerController2D
    {
        [Header("Testing")]
        [SerializeField, Tooltip("Turn off to run the original PlayerController2D movement instead. Can be toggled in play mode with P or R3")]
        private bool useNewPhysics = true;

        private InputAction togglePhysicsAction;
        private Action originalFixedUpdate;

        private readonly PlayerPhysicsContext context = new PlayerPhysicsContext();
        private PlayerPhysicsModule[] modules;
        private Rigidbody2D body;
        private PlayerInputHandler inputHandler;
        private bool initialized;
        private bool jumpTriggered;

        private Vector2 AppliedExtraSpeed => new Vector2(0f, ExtraSpeed.y);

        private void Awake()
        {
            togglePhysicsAction = new InputAction("TogglePhysics", InputActionType.Button);
            togglePhysicsAction.AddBinding("<Keyboard>/p");
            togglePhysicsAction.AddBinding("<Gamepad>/rightStickButton");

            MethodInfo baseFixedUpdate = typeof(PlayerController2D).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
            originalFixedUpdate = (Action)Delegate.CreateDelegate(typeof(Action), this, baseFixedUpdate);
        }

        private void OnEnable() => togglePhysicsAction.Enable();

        private void OnDisable() => togglePhysicsAction.Disable();

        private void OnDestroy() => togglePhysicsAction.Dispose();

        private void Update()
        {
            if (togglePhysicsAction.WasPressedThisFrame() && Time.timeScale > 0f && !LockedOrNot())
            {
                SetNewPhysics(!useNewPhysics);
            }
        }

        private void FixedUpdate()
        {
            if (!initialized)
            {
                Initialize();
            }

            if (!useNewPhysics)
            {
                originalFixedUpdate();
                return;
            }

            FlagUpdates();

            Vector2 velocity = KeepPhysicsSlowdown(InternalVelocity, body.linearVelocity - AppliedExtraSpeed - ExternalVelocity());
            bool wasGrounded = context.IsGrounded;
            bool grounded = velocity.y <= 0.1f && TouchingGround();
            if (grounded && !OnMovingPlatform)
            {
                ExtraSpeed = Vector2.zero;
            }

            context.BeginStep(velocity, grounded, PlayerRef.PlayerManager.PlayerStats, inputHandler, jumpTriggered, Time.fixedDeltaTime);
            jumpTriggered = false;

            if (grounded && !wasGrounded)
            {
                PlayLandingNoise();
            }

            foreach (PlayerPhysicsModule module in modules)
            {
                if (module.isActiveAndEnabled && module.UpdateMovement(context))
                {
                    break;
                }
            }

            foreach (PlayerPhysicsModule module in modules)
            {
                if (module.isActiveAndEnabled)
                {
                    module.ConstrainMovement(context);
                }
            }

            currentState = context.State;
            InternalVelocity = context.Velocity;
            CombineCurrentVelocities();
            context.EndStep();
        }

        public override void CombineCurrentVelocities()
        {
            base.CombineCurrentVelocities();

            if (useNewPhysics && initialized)
            {
                body.linearVelocity += ExternalVelocity();
            }
        }

        public override void TriggerJump()
        {
            if (useNewPhysics)
            {
                jumpTriggered = true;
            }
            else
            {
                base.TriggerJump();
            }
        }

        private void Initialize()
        {
            body = GetComponent<Rigidbody2D>();
            inputHandler = GetComponent<PlayerInputHandler>();
            modules = GetComponents<PlayerPhysicsModule>().OrderBy(module => module.Order).ToArray();
            foreach (PlayerPhysicsModule module in modules)
            {
                module.Initialize(this);
            }

            LevelManager.OnPlayerReset.AddListener(ResetMovement);
            LevelManager.OnPlayerDeath.AddListener(ResetMovement);

            initialized = true;
        }

        /// <summary>
        /// Switches between the new physics and the original PlayerController2D movement on the same component,
        /// handing over the current velocity and wire state so the player keeps moving from where they are.
        /// </summary>
        private void SetNewPhysics(bool enable)
        {
            if (!initialized)
            {
                Initialize();
            }

            useNewPhysics = enable;
            ResetMovement();
            InternalVelocity = body.linearVelocity - AppliedExtraSpeed - (enable ? ExternalVelocity() : Vector2.zero);
            currentState = PlayerState.Aerial;

            if (!enable)
            {
                foreach (PlayerPhysicsModule module in modules)
                {
                    module.OnNewPhysicsDisabled();
                }
            }
        }

        private void ResetMovement()
        {
            context.Reset();
            jumpTriggered = false;
            foreach (PlayerPhysicsModule module in modules)
            {
                module.ResetState();
            }
        }

        /// <summary>
        /// Collisions can slow the player down, but scripts that change the rigidbody's velocity directly (e.g. moving
        /// platforms when the player steps on or off them) must not add speed, so only reductions of the velocity
        /// commanded last step are kept.
        /// </summary>
        private static Vector2 KeepPhysicsSlowdown(Vector2 commanded, Vector2 actual)
        {
            return new Vector2(Slowdown(commanded.x, actual.x), Slowdown(commanded.y, actual.y));

            static float Slowdown(float wanted, float result) =>
                wanted > 0 ? Mathf.Clamp(result, 0f, wanted) : Mathf.Clamp(result, wanted, 0f);
        }
    }
}
