namespace Levels.Objects.Platform
{
    using UnityEngine;
    using CharacterController;
    /// <summary>
    /// Represents the behavior of a platform object.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;
        protected virtual void OnCollisionEnter2D(Collision2D col)
        {
            if (IsOnTop(col.GetContact(0).normal))
            {
                col.transform.SetParent(transform);
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            collision.transform.SetParent(null);
        }
    }
}
