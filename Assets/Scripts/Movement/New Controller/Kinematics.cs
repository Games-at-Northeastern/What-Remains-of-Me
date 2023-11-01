using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematics
{

    #region Velocity
    /// <summary>
    ///     calculates velocity based on initial Velocity and acceleration
    /// </summary>

    public static float Velocity(float initialVelocity, float acceleration, float time)
    {
        return initialVelocity + acceleration * time;
    }
    /// <summary>
    /// calculates velocity based on acceleration, distance, and initial Velocity
    /// </summary>
    /// <param name="finalVelocity"></param>
    /// <param name="acceleration"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float InitialVelocity(float finalVelocity, float acceleration, float distance)
    {
        return Mathf.Sqrt(Mathf.Pow(finalVelocity, 2) - (2 * acceleration * distance));
    }
    /// <summary>
    /// calculates velocity based on initial Velocity and acceleration caps at target
    /// not exceeding acceleration in chage in velocity
    /// if accelerating already outside of target bounds will continue going further away 
    /// </summary>

    public static float VelocityTarget(float initialVelocity, float acceleration, float target, float time)
    {
        float directionToTarget = Mathf.Sign(target - initialVelocity);
        float newSpeed = Velocity(initialVelocity, acceleration, time);
        if (Mathf.Sign(acceleration) == directionToTarget)
        {
            if (directionToTarget < 0)
            {
                if (newSpeed < target)
                {
                    return target;
                }
                else
                {
                    return newSpeed;
                }
            }
            else
            {
                if (newSpeed > target)
                {
                    return target;
                }
                else
                {
                    return newSpeed;
                }
            }
        }
        else
        {
            return newSpeed;
        }
    }
    /// <summary>
    /// calculates the velocity and clamps it to + or minus target
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <param name="acceleration"></param>
    /// <param name="target"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static float VelocityClamp(float initialVelocity, float acceleration, float target, float time)
    {
        target = Mathf.Abs(target);

        return Mathf.Clamp(Velocity(initialVelocity, acceleration, time), -target, target);
    }
    /// <summary>
    /// Calculates change in velocity based on delta time
    /// </summary>

    public static Vector2 DeltaVelocity(Vector2 acceleration, float time)
    {
        return acceleration * time;
    }
    /// <summary>
    /// Calculates change in velocity based on delta time
    /// </summary>
    /// <param name="acceleration"></param>
    /// <param name="time"></param>
    /// <returns></returns>

    public static float DeltaVelocity(float acceleration, float time)
    {
        return acceleration * time;
    }
    #endregion

    #region Acceleration
    public static Vector2 Acceleration(Vector2 force, float mass)
    {
        return new Vector2(force.x / mass, force.y / mass);
    }

    #endregion
    #region Distance
    public static float Distance(float initialVelocity, float acceleration, float time)
    {
        return time * (initialVelocity + (acceleration * time));
    }
    #endregion
}
