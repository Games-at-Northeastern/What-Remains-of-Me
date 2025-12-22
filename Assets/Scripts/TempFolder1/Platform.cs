namespace Levels.Objects.Platform
{
    using UnityEngine;
    using CharacterController;
    /// <summary>
    /// Represents the behavior of a platform object.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [SerializeField] private string _eventCollisionTag = "PlatformEventTarget";

        protected virtual bool IsOnTop(Vector2 normal) => Vector2.Dot(transform.up, normal) < -0.5f;

        protected virtual void OnCollisionEnter2D(Collision2D col)
        {

            if (IsOnTop(col.GetContact(0).normal))
            {
                col.transform.SetParent(transform);
            }


            // platform behavior was deleted by a merge resolution; this should work?
            if (!string.IsNullOrEmpty(_eventCollisionTag) && col.gameObject.CompareTag(_eventCollisionTag)) {
                var coll = col.gameObject.GetComponent<IOnCollision>();

                if (coll != null)
                {
                    coll.Collide((col.transform.position - transform.position).normalized);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            collision.transform.SetParent(null);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsOnTop(other.transform.position) && other.gameObject.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
            }

            if (!string.IsNullOrEmpty(_eventCollisionTag) && other.CompareTag(_eventCollisionTag))
            {
                var triggerHandler = other.GetComponent<IOnCollision>();
                if (triggerHandler != null)
                {
                    triggerHandler.Collide((other.transform.position - transform.position).normalized);
                }
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.transform.parent == transform)
            {
                other.transform.SetParent(null);
            }
        }
    }
}
