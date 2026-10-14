using System;
using System.Collections.Generic;
using UnityEngine;
using static SwingAnimUtil;

[CreateAssetMenu(fileName = "SwingAnimationData", menuName = "Scriptable Objects/Swing Animation Data")]
public class SwingAnimationData : ScriptableObject
{
    [SerializeField] Sprite[] swingRightFromCenterFacingRight;
    [SerializeField] Sprite[] swingRightFromCenterFacingLeft;
    [SerializeField] Sprite[] swingLeftFromRightFacingRight;
    [SerializeField] Sprite[] swingLeftFromRightFacingLeft;
    [SerializeField] Sprite[] idleOnRightFacingRight;
    [SerializeField] Sprite[] idleOnRightFacingLeft;
    [SerializeField] Sprite[] idleCenterFacingRight;

    /*
    Swing to right (face right and face left)
    Swing from right (face right and face left)
    Idle right (face right and face left)
    Idle center (face right)
    */
    // If I want swing left from center facing left I can flip swing right from center facing right
    // If I want swing left from center facing right I can flip swing right from center facing left

    public Sprite[] GetFrames(SwingAnim animType, bool facingRight, out bool flipSprite)
    {
        switch (animType)
        {
            case SwingAnim.SWING_RIGHT_FROM_CENTER:
                flipSprite = false;
                if (facingRight)
                    return swingRightFromCenterFacingRight;
                else
                    return swingRightFromCenterFacingLeft;
            case SwingAnim.SWING_LEFT_FROM_CENTER:
                flipSprite = true;
                if (facingRight)
                    return swingRightFromCenterFacingLeft;
                else
                    return swingRightFromCenterFacingRight;
            case SwingAnim.SWING_LEFT_FROM_RIGHT:
                flipSprite = false;
                if (facingRight)
                    return swingLeftFromRightFacingRight;
                else
                    return swingLeftFromRightFacingLeft;
            case SwingAnim.SWING_RIGHT_FROM_LEFT:
                flipSprite = true;
                if (facingRight)
                    return swingLeftFromRightFacingLeft;
                else
                    return swingLeftFromRightFacingRight;
            case SwingAnim.IDLE_RIGHT:
                flipSprite = false;
                if (facingRight)
                    return idleOnRightFacingRight;
                else
                    return idleOnRightFacingLeft;
            case SwingAnim.IDLE_LEFT:
                flipSprite = true;
                if (facingRight)
                    return idleOnRightFacingLeft;
                else
                    return idleOnRightFacingRight;
            default: // aka idle center
                flipSprite = !facingRight;
                return idleCenterFacingRight;
        }
    }

    public int FrameCount(SwingAnim animType)
    {
        switch (animType)
        {
            case SwingAnim.SWING_RIGHT_FROM_CENTER:
            case SwingAnim.SWING_LEFT_FROM_CENTER:
                return swingRightFromCenterFacingRight.Length;
            case SwingAnim.SWING_LEFT_FROM_RIGHT:
            case SwingAnim.SWING_RIGHT_FROM_LEFT:
                return swingLeftFromRightFacingLeft.Length;
        }
        // all idle anims have same length
        return idleOnRightFacingLeft.Length;
    }
}