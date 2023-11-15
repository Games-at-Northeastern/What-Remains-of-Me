namespace Levels.Objects.Platform
{
    using UnityEngine;

    /// <summary>
    /// Represents the behavior of a platform object.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [SerializeField] private string _eventCollisionTag; // tag for object we collide with to trigger its event

        private Rigidbody2D rb;
        private MovementExecuter movementExecuter;

        private void Awake()
        {
            movementExecuter = GameObject.FindGameObjectsWithTag("Player")[0].GetComponentInChildren<MovementExecuter>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                /*
                 * // TODO : this should NOT have an explicit reference to the player's movement executor...
                 * 
                 * Meh. Probably.
                */

                movementExecuter.isOnAPlatform = true;
                movementExecuter.platformRb = rb;
            }
            else if (collision.gameObject.CompareTag(_eventCollisionTag))
            {
                var col = collision.gameObject.GetComponent<IOnCollision>();

                if (col != null)
                {
                    col.Collide((collision.transform.position - transform.position).normalized);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                movementExecuter.isOnAPlatform = false;
            }
        }
    }
}
