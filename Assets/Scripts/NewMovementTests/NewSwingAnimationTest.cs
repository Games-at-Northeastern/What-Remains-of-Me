using System.Collections;
using CharacterController;
using UnityEngine;
using static SwingAnimUtil;

/// <summary>
/// Test-only copy of SwingAnimation that follows NewHookTest and NewPlayerControllerTest instead of the
/// WireThrower and PlayerController2D. Uses the same swing frames and angles as the player prefab.
/// </summary>
public class NewSwingAnimationTest : MonoBehaviour
{
    [SerializeField] private NewPlayerControllerTest player;
    [SerializeField] private NewHookTest hook;
    [SerializeField] private SpriteRenderer playerRend;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteFlipper spriteFlipper;
    [Space]
    [SerializeField] private SwingAnimationData data;
    [SerializeField] private Sprite mainSprite;
    [SerializeField] private float centerAngle;
    [SerializeField] private float legAngle;
    [Space]
    [SerializeField] private int targetFPS = 30;

    private bool shouldUpdate;
    private bool animatorHasControl;
    private bool wasAttached;

    private bool facingRight;
    private bool swingingRight;
    private SwingSlice slice;

    private bool wasSwingingRight;
    private SwingSlice lastSlice = SwingSlice.CENTER;
    private float lastPlayerX;

    private Coroutine activeRoutine;

    private bool swingToCenterAutoplays;

    private void Update()
    {
        if (hook.IsAttached != wasAttached)
        {
            wasAttached = hook.IsAttached;
            if (wasAttached)
            {
                BeginGrapple();
            }
            else
            {
                EndGrapple();
            }
        }

        if (!shouldUpdate)
        {
            return;
        }

        lastSlice = slice;
        slice = GetSwingSlice();

        if (ShouldAnimatorHandle())
        {
            if (!animatorHasControl)
            {
                if (activeRoutine != null)
                {
                    StopCoroutine(activeRoutine);
                }
                GiveAnimatorControl();
            }
        }
        else if (animatorHasControl)
        {
            BeginGrapple();
        }

        if (lastSlice != slice)
        {
            SliceChanged(lastSlice, slice);
        }

        wasSwingingRight = swingingRight;
        swingingRight = lastPlayerX < playerRend.gameObject.transform.position.x;
        if (wasSwingingRight != swingingRight && slice is SwingSlice.LEFT_LEG or SwingSlice.RIGHT_LEG)
        {
            DirectionChanged();
        }

        facingRight = player.LeftOrRight is Facing.right;
    }

    private SwingSlice GetSwingSlice()
    {
        Vector3 outletPos = hook.AnchorPosition;
        Vector3 outletPosCentered = outletPos - player.transform.position;
        float wireLineAngle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(outletPosCentered.y) / Mathf.Abs(outletPosCentered.x));
        float playerOutletAngle = 90f - wireLineAngle;

        if (playerOutletAngle > 90f)
        {
            return SwingSlice.UPPER_HALF;
        }
        if (playerOutletAngle * 2f < centerAngle)
        {
            return SwingSlice.CENTER;
        }

        float playerX = player.transform.position.x;
        if (playerOutletAngle > centerAngle + legAngle)
        {
            return playerX > outletPos.x ? SwingSlice.RIGHT_WING : SwingSlice.LEFT_WING;
        }
        return playerX > outletPos.x ? SwingSlice.RIGHT_LEG : SwingSlice.LEFT_LEG;
    }

    private void SliceChanged(SwingSlice last, SwingSlice current)
    {
        if (last == current)
        {
            return;
        }

        if (current is SwingSlice.RIGHT_WING or SwingSlice.LEFT_WING)
        {
            swingToCenterAutoplays = true;
        }

        if (last is SwingSlice.CENTER)
        {
            swingToCenterAutoplays = false;
            if (current is SwingSlice.RIGHT_LEG)
            {
                Play(SwingAnim.SWING_RIGHT_FROM_CENTER);
            }
            else if (current is SwingSlice.LEFT_LEG)
            {
                Play(SwingAnim.SWING_LEFT_FROM_CENTER);
            }
        }
        else if (current is SwingSlice.CENTER)
        {
            swingToCenterAutoplays = false;
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            playerRend.sprite = mainSprite;
            playerRend.flipX = !facingRight;
        }
        else if (last is SwingSlice.LEFT_WING)
        {
            Play(SwingAnim.SWING_RIGHT_FROM_LEFT);
        }
        else if (last is SwingSlice.RIGHT_WING)
        {
            Play(SwingAnim.SWING_LEFT_FROM_RIGHT);
        }
    }

    private void DirectionChanged()
    {
        if (slice is SwingSlice.CENTER)
        {
            playerRend.flipX = !facingRight;
        }

        if (!swingToCenterAutoplays)
        {
            if (slice is SwingSlice.LEFT_LEG)
            {
                Play(SwingAnim.SWING_RIGHT_FROM_LEFT);
            }
            else if (slice is SwingSlice.RIGHT_LEG)
            {
                Play(SwingAnim.SWING_LEFT_FROM_RIGHT);
            }
        }
    }

    private void BeginGrapple()
    {
        animatorHasControl = false;
        spriteFlipper.enabled = false;
        playerAnimator.enabled = false;
        player.transform.rotation = Quaternion.identity;
        playerRend.sprite = mainSprite;
        shouldUpdate = true;
    }

    private void EndGrapple()
    {
        GiveAnimatorControl();
        shouldUpdate = false;
    }

    private void GiveAnimatorControl()
    {
        animatorHasControl = true;
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        playerRend.flipX = false;
        spriteFlipper.enabled = true;
        player.transform.rotation = new Quaternion(0f, facingRight ? 0f : 180f, 0f, 0f);
        playerAnimator.enabled = true;
    }

    private bool ShouldAnimatorHandle() => player.Grounded || slice == SwingSlice.UPPER_HALF;

    private void Play(SwingAnim anim, bool autoPlayIdle = true, int startFrame = 0)
    {
        if (startFrame >= data.FrameCount(anim) || startFrame < 0)
        {
            return;
        }
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(PlayFrames(anim, autoPlayIdle, startFrame));
    }

    private void Loop(SwingAnim anim, int startFrame = 0)
    {
        if (startFrame >= data.FrameCount(anim) || startFrame < 0)
        {
            return;
        }
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }
        activeRoutine = StartCoroutine(LoopFrames(anim, startFrame));
    }

    private IEnumerator PlayFrames(SwingAnim anim, bool autoPlayIdle, int startFrame)
    {
        Sprite[] frames;
        bool facingRightWhenGotFrames = facingRight;
        GetFrames();

        for (int i = startFrame; i < data.FrameCount(anim); i++)
        {
            playerRend.sprite = frames[i];
            yield return new WaitForSeconds(1f / targetFPS);
            if (facingRightWhenGotFrames != facingRight)
            {
                GetFrames();
            }
        }

        if (autoPlayIdle && !anim.IsIdle())
        {
            Loop(anim.ToIdleAnim());
        }

        void GetFrames()
        {
            facingRightWhenGotFrames = facingRight;
            frames = data.GetFrames(anim, facingRight, out bool flipSprite);
            playerRend.flipX = flipSprite;
        }
    }

    private IEnumerator LoopFrames(SwingAnim anim, int startFrame)
    {
        Sprite[] frames;
        bool facingRightWhenGotFrames = facingRight;
        GetFrames();

        int idx = startFrame;
        while (enabled)
        {
            if (frames[idx] != null)
            {
                playerRend.sprite = frames[idx];
                yield return new WaitForSeconds(1f / targetFPS);
            }
            idx = (idx + 1) % frames.Length;
            if (facingRightWhenGotFrames != facingRight)
            {
                GetFrames();
            }
        }

        void GetFrames()
        {
            facingRightWhenGotFrames = facingRight;
            frames = data.GetFrames(anim, facingRight, out bool flipSprite);
            playerRend.flipX = flipSprite;
        }
    }

    private enum SwingSlice { RIGHT_WING, RIGHT_LEG, CENTER, LEFT_LEG, LEFT_WING, UPPER_HALF }
}
