using System;
using System.Collections;
using PlayerController;
using UnityEngine;
using static SwingAnimUtil;

public class SwingAnimation : MonoBehaviour
{
    [SerializeField] PlayerController2D player;
    [SerializeField] SpriteRenderer playerRend;
    [SerializeField] Animator playerAnimator;
    [SerializeField] SpriteFlipper spriteFlipper;
    [SerializeField] WireThrower wire;
    [Space]
    [SerializeField] SwingAnimationData data;
    [SerializeField] Sprite mainSprite;
    [SerializeField] float centerAngle;
    [SerializeField] float legAngle;
    [Space]
    [SerializeField] int targetFPS = 30;

    bool shouldUpdate;
    bool animatorHasControl;

    bool facingRight;
    bool swingingRight;
    SwingSlice slice;

    bool wasSwingingRight;
    SwingSlice lastSlice = SwingSlice.CENTER;
    float lastPlayerX;

    Coroutine activeRoutine;

    bool swingToCenterAutoplays = false;

    void Awake()
    {
        wire.onConnect.AddListener(BeginGrapple);
        wire.onDisconnect.AddListener(EndGrapple);
    }

    void Update()
    {
        if (!shouldUpdate)
            return;

        lastSlice = slice;
        slice = GetSwingSlice();

        bool shouldAnimatorHandle = ShouldAnimatorHandle();
        if (shouldAnimatorHandle)
        {
            if (!animatorHasControl)
            {
                StopCoroutine(activeRoutine);
                GiveAnimatorControl();
            }
        }
        else
        {
            if (animatorHasControl)
            {
                BeginGrapple();
            }
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

        facingRight = player.LeftOrRight is CharacterController.Facing.right;
    }

    SwingSlice GetSwingSlice()
    {
        Vector3 outletPos = wire.ConnectedOutlet.transform.position;
        Vector3 outletPosCentered = outletPos - player.transform.position;
        float wireLineAngle = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(outletPosCentered.y) / Mathf.Abs(outletPosCentered.x));
        float playerOutletAngle = 90f - wireLineAngle;

        if (playerOutletAngle > 90f)
            return SwingSlice.UPPER_HALF;

        if (playerOutletAngle * 2f < centerAngle)
            return SwingSlice.CENTER;
        else
        {
            float playerX = player.transform.position.x;
            if (playerOutletAngle > centerAngle + legAngle)
                return playerX > outletPos.x ? SwingSlice.RIGHT_WING : SwingSlice.LEFT_WING;
            else
                return playerX > outletPos.x ? SwingSlice.RIGHT_LEG : SwingSlice.LEFT_LEG;
        }
    }

    void SliceChanged(SwingSlice last, SwingSlice current)
    {
        if (last == current)
            return;

        if (current is SwingSlice.RIGHT_WING or SwingSlice.LEFT_WING)
        {
            swingToCenterAutoplays = true;
        }

        if (last is SwingSlice.CENTER)
        {
            swingToCenterAutoplays = false;

            if (current is SwingSlice.RIGHT_LEG)
                Play(SwingAnim.SWING_RIGHT_FROM_CENTER);
            else if (current is SwingSlice.LEFT_LEG)
                Play(SwingAnim.SWING_LEFT_FROM_CENTER);
        }

        else if (current is SwingSlice.CENTER)
        {
            swingToCenterAutoplays = false;
            StopCoroutine(activeRoutine);
            playerRend.sprite = mainSprite;
            playerRend.flipX = !facingRight;
        }
        else if (last is SwingSlice.LEFT_WING)
            Play(SwingAnim.SWING_RIGHT_FROM_LEFT);

        else if (last is SwingSlice.RIGHT_WING)
            Play(SwingAnim.SWING_LEFT_FROM_RIGHT);
    }

    void DirectionChanged()
    {
        if (slice is SwingSlice.CENTER)
        {
            playerRend.flipX = !facingRight;
        }
        if (!swingToCenterAutoplays)
        {
            if (slice is SwingSlice.LEFT_LEG)
                Play(SwingAnim.SWING_RIGHT_FROM_LEFT);
            else if (slice is SwingSlice.RIGHT_LEG)
                Play(SwingAnim.SWING_LEFT_FROM_RIGHT);
        }
    }

    void BeginGrapple()
    {
        animatorHasControl = false;
        spriteFlipper.enabled = false;
        playerAnimator.enabled = false;
        player.transform.rotation = Quaternion.identity;

        playerRend.sprite = mainSprite;

        shouldUpdate = true;
    }

    void EndGrapple()
    {
        animatorHasControl = true;
        GiveAnimatorControl();
        shouldUpdate = false;
    }

    void GiveAnimatorControl()
    {
        animatorHasControl = true;
        if (activeRoutine != null)
            StopCoroutine(activeRoutine);
        playerRend.flipX = false;
        spriteFlipper.enabled = true;
        player.transform.rotation = new Quaternion(0f, facingRight ? 0f : 180f, 0f, 0f);
        playerAnimator.enabled = true;
    }

    bool ShouldAnimatorHandle()
    {
        return player.Grounded || slice == SwingSlice.UPPER_HALF;
    }

    // ANIMATIONS

    private void Play(SwingAnim anim, bool autoPlayIdle = true, int startFrame = 0)
    {
        if (startFrame >= data.FrameCount(anim) || startFrame < 0)
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(PlayFrames(anim, autoPlayIdle, startFrame));
    }

    private void Loop(SwingAnim anim, int startFrame = 0)
    {
        if (startFrame >= data.FrameCount(anim) || startFrame < 0)
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        activeRoutine = StartCoroutine(LoopFrames(anim, startFrame));
    }

    private IEnumerator PlayFrames(SwingAnim anim, bool autoPlayIdle = true, int startFrame = 0)
    {
        Sprite[] frames;
        bool facingRightWhenGotFrames = facingRight;
        GetFrames();

        for (int i = startFrame; i < data.FrameCount(anim); i++)
        {
            playerRend.sprite = frames[i];
            yield return new WaitForSeconds(1f / targetFPS);

            if (facingRightWhenGotFrames != facingRight)
                GetFrames();
        }

        if (autoPlayIdle && !anim.IsIdle())
        {
            Loop(anim.ToIdleAnim());
        }

        void GetFrames()
        {
            facingRightWhenGotFrames = facingRight;
            frames = data.GetFrames(anim, facingRight, out var flipSprite);
            playerRend.flipX = flipSprite;
        }
    }

    private IEnumerator LoopFrames(SwingAnim anim, int startFrame = 0)
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
                GetFrames();
        }

        void GetFrames()
        {
            facingRightWhenGotFrames = facingRight;
            frames = data.GetFrames(anim, facingRight, out var flipSprite);
            playerRend.flipX = flipSprite;
        }
    }

    enum SwingSlice { RIGHT_WING, RIGHT_LEG, CENTER, LEFT_LEG, LEFT_WING, UPPER_HALF };
}

public static class SwingAnimUtil
{
    public enum SwingAnim
    {
        SWING_RIGHT_FROM_CENTER, // have
        SWING_RIGHT_FROM_LEFT,
        SWING_LEFT_FROM_CENTER,
        SWING_LEFT_FROM_RIGHT, // have
        IDLE_RIGHT, // have
        IDLE_LEFT,
        IDLE_CENTER, // half have
    }

    public static bool IsIdle(this SwingAnim anim) => anim is SwingAnim.IDLE_RIGHT
        or SwingAnim.IDLE_LEFT
        or SwingAnim.IDLE_CENTER;

    public static SwingAnim ToIdleAnim(this SwingAnim anim)
    {
        if (anim.IsIdle())
            return anim;

        return anim switch
        {
            SwingAnim.SWING_RIGHT_FROM_CENTER => SwingAnim.IDLE_RIGHT,
            SwingAnim.SWING_RIGHT_FROM_LEFT => SwingAnim.IDLE_CENTER,
            SwingAnim.SWING_LEFT_FROM_CENTER => SwingAnim.IDLE_LEFT,
            SwingAnim.SWING_LEFT_FROM_RIGHT => SwingAnim.IDLE_CENTER,
        };
    }

    public static SwingAnim FromIdleAnim(this SwingAnim anim)
    {
        if (anim.IsIdle())
            return anim;

        return anim switch
        {
            SwingAnim.SWING_RIGHT_FROM_CENTER => SwingAnim.IDLE_CENTER,
            SwingAnim.SWING_RIGHT_FROM_LEFT => SwingAnim.IDLE_LEFT,
            SwingAnim.SWING_LEFT_FROM_CENTER => SwingAnim.IDLE_CENTER,
            SwingAnim.SWING_LEFT_FROM_RIGHT => SwingAnim.IDLE_RIGHT,
        };
    }

    public static bool TryGetToCenter(this SwingAnim anim, out SwingAnim result)
    {
        result = anim;
        if (anim.EndsAtCenter())
            return false;

        switch (anim)
        {
            case SwingAnim.SWING_RIGHT_FROM_CENTER:
            case SwingAnim.IDLE_RIGHT:
                result = SwingAnim.SWING_LEFT_FROM_RIGHT;
                break;
            case SwingAnim.SWING_LEFT_FROM_CENTER:
            case SwingAnim.IDLE_LEFT:
                result = SwingAnim.SWING_RIGHT_FROM_LEFT;
                break;
        }

        return true;
    }

    public static bool EndsAtCenter(this SwingAnim anim) => anim is SwingAnim.IDLE_CENTER
            or SwingAnim.SWING_LEFT_FROM_RIGHT
            or SwingAnim.SWING_RIGHT_FROM_LEFT;
}
