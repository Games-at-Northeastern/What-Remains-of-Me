using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
using More2DGizmos;
namespace PlayerControllerRefresh
{
    public class PlayerController : MonoBehaviour, ICharacterController


    {
        #region Collision
        public bool TouchingLeftWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);

            return GizmosPlus.BoxCast(origin + new Vector2(-leftOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.left, sideBounds.x - 0.01f, ~playerLayer);

        }
        public bool TouchingRightWall()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(rightOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.right, sideBounds.x - 0.01f, ~playerLayer);
        }
        public bool TouchingCeiling()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(0, ceilingOffset), new Vector2(ceilingBounds.x, 0.01f), 0, Vector2.up, ceilingBounds.y - 0.01f, ~playerLayer);
        }
        public bool TouchingGround()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            return GizmosPlus.BoxCast(origin + new Vector2(0, -groundOffset), new Vector2(groundBounds.x, 0.01f), 0, Vector2.down, groundBounds.y - 0.01f, ~playerLayer);

        }
        #endregion


        public Vector2 Speed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public Vector2 Velocity => throw new System.NotImplementedException();

        public Vector3 position => throw new System.NotImplementedException();

        public bool Grounded => throw new System.NotImplementedException();

        public Vector2 Direction => throw new System.NotImplementedException();

        public Facing LeftOrRight => throw new System.NotImplementedException();

        public void LockInputs() => throw new System.NotImplementedException();
        public void UnlockInputs() => throw new System.NotImplementedException();

        public PlayerSettings settings;
        [HideInInspector] public CapsuleCollider2D col;
        public LayerMask playerLayer;
        GroundMovement groundMovement;
        AirMovement airMovement;
        Jump jump;
        Dash dash;
        Swing swing;
        Knockback knockback;
        WallJump wallJump;
        WallSlide wallSlide;

        PlayerInputHandler playerInputs;
#if UNITY_EDITOR
        private void OnValidate()
        {
            Start();
        }
#endif

        // Start is called before the first frame update
        void Start()
        {
            col = GetComponent<CapsuleCollider2D>();
            groundMovement = new GroundMovement(settings.maxRunSpeed, settings.groundedAcceleration, settings.groundedDeceleration, this);
        }

        // Update is called once per frame
        void Update()
        {
            groundMovement.UpdateDirection(playerInputs.MoveInput.x);
            groundMovement.ContinueMove();
        }
        [Header("CollisionDetectors")]
        [Tooltip("X is the width Y is the height")]
        [SerializeField] private Vector2 ceilingBounds;
        [Min(0)]
        [SerializeField] private float ceilingOffset;

        [Tooltip("X is the width Y is the height")]
        [SerializeField] Vector2 sideBounds;
        [Min(0)]
        [SerializeField] private float leftOffset;
        [SerializeField] private float rightOffset;

        [Tooltip("X is the width Y is the height")]
        [SerializeField] private Vector2 groundBounds;
        [Min(0)]
        [SerializeField] private float groundOffset;


        private void OnDrawGizmos()
        {
            Vector2 origin = Kinematics.CapsuleColliderCenter(col);
            GizmosPlus.BoxCast(origin + new Vector2(rightOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.right, sideBounds.x - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(-leftOffset, 0), new Vector2(0.01f, sideBounds.y), 0, Vector2.left, sideBounds.x - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(0, ceilingOffset), new Vector2(ceilingBounds.x, 0.01f), 0, Vector2.up, ceilingBounds.y - 0.01f, ~playerLayer);
            GizmosPlus.BoxCast(origin + new Vector2(0, -groundOffset), new Vector2(groundBounds.x, 0.01f), 0, Vector2.down, groundBounds.y - 0.01f, ~playerLayer);
        }
    }
}
