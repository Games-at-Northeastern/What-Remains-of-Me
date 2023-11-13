using UnityEngine;

public class Kinematics
{


    #region Velocity
    //calculates velocity based on initial Velocity and acceleration
    public static float Velocity(float initialVelocity, float acceleration, float time)
    {
        return initialVelocity + acceleration * time;
    }
    //calculates velocity based on acceleration, distance, and initial Velocity
    public static float Velocity2(float initialVelocity, float acceleration, float distance)
    {
        return Mathf.Sqrt(Mathf.Pow(initialVelocity, 2) + 2 * acceleration * distance);
    }
    public static float InitialVelocity(float finalVelocity, float acceleration, float distance)
    {
        return Mathf.Sqrt(Mathf.Pow(finalVelocity, 2) - (2 * acceleration * distance));
    }
    //calculates velocity based on initial Velocity and acceleration caps at target
    public static float VelocityTarget(float initialVelocity, float acceleration, float target, float time)
    {
        float directionToTarget = Mathf.Sign(target - initialVelocity);
        float newSpeed = Velocity(initialVelocity, acceleration, time);
        if (Mathf.Sign(acceleration) == directionToTarget)
        {
            if (directionToTarget < 0)
            {
                return newSpeed < target ? target : newSpeed;
            }
            else
            {
                return newSpeed > target ? target : newSpeed;
            }
        }
        else
        {
            return newSpeed;
        }
    }
    //moves velocity towards target
    public static float VelocityTowards(float initialVelocity, float acceleration, float target, float time)
    {
        return Mathf.MoveTowards(initialVelocity, target, acceleration * time);
    }
    //calculates velocity based on force and mass
    public static Vector2 DeltaVelocity(Vector2 force, float mass, float time)
    {
        return DeltaVelocity(Acceleration(force, mass), time);
    }
    //Calculates change in velocity based on delta time
    public static Vector2 DeltaVelocity(Vector2 acceleration, float time)
    {
        return acceleration * time;
    }
    //Calculates change in velocity based on delta time
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

    #region Vectors
    /// <summary>
    /// Returns a vector of given magnitude with given angle with respect to a vector(1,0)
    /// </summary>
    /// <param name="magnitude"> the vectors length</param>
    /// <param name="angle"> angle made with vector(1,0)</param>
    /// <returns></returns>
    public static Vector2 GetVector(float magnitude, float angle)
    {
        return RotateVector(new Vector2(magnitude, 0), angle);
    }
    /// <summary>
    ///     rotates vector counterclockwise by given euler angle
    /// </summary>

    public static Vector2 RotateVector(Vector2 vec, float angle)
    {

        float x, y;
        x = vec.x * Mathf.Cos(Mathf.Deg2Rad * angle) - vec.y * Mathf.Sin(Mathf.Deg2Rad * angle);
        y = vec.x * Mathf.Sin(Mathf.Deg2Rad * angle) - vec.y * Mathf.Cos(Mathf.Deg2Rad * angle);
        return new Vector2(x, y);
    }
    #endregion

    #region Angles
    //Returns the angle of a Vector
    public static float Direction(Vector2 vector)
    {
        return Vector2.Angle(Vector2.right, vector);
    }
    public static float SignedEulerAngle(Vector2 normal)
    {
        return Vector2.SignedAngle(Vector2.right, normal);
    }
    //returns angle parralel to ground
    public static float SignedEulerGroundAngle(Vector2 GroundNormal)
    {
        return SignedEulerGroundAngle(GroundNormal, 1);
    }
    //returns angle parralel to ground if ground is flat use direction to determine rather it is 180 degrees or 0 degrees
    public static float SignedEulerGroundAngle(Vector2 GroundNormal, int direction)
    {
        if (GroundNormal.x == 0)
        {
            if (direction > 0)
            {
                return 0;
            }
            else if (direction < 0)
            {
                return 180;
            }
            else
            {
                Debug.LogWarning("direction should be a Non-ZeroInteger");
                return 0;
            }
        }
        return Direction(GroundNormal) + Mathf.Sign(GroundNormal.x) * 90;
    }
    #endregion

    public static Vector2 CapsuleColliderCenter(CapsuleCollider2D col)
    {
        GameObject obj = col.gameObject;
        Vector2 pos = obj.transform.position;
        float rotation = obj.transform.eulerAngles.z;
        return pos + Kinematics.GetVector(col.offset.y, rotation + 90) + Kinematics.GetVector(col.offset.x, rotation);
    }
}
