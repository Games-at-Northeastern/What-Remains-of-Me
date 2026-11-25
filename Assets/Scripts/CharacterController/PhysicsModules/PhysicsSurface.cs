using UnityEngine;

namespace PlayerController
{
    public class PhysicsSurface : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float accelerationMultiplier = 1f;
        [SerializeField, Min(0f)] private float decelerationMultiplier = 1f;

        public float AccelerationMultiplier => accelerationMultiplier;
        public float DecelerationMultiplier => decelerationMultiplier;
    }
}
