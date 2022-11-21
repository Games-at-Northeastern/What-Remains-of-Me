using UnityEngine;

/// <summary>
/// Allows designers to configure responses to OnCollisionEnter, 
/// OnCollisionStay, and OnCollisionExit with a specific tag all in the 
/// inspector.
/// </summary>
public class CollisionDataRelay2D : MonoBehaviour
{
    /// <summary>
    /// Collection of responses to a collision.
    /// </summary>
    [Tooltip("Collection of responses to a collision.")]
    [SerializeField] private CollisionResponse[] _collisionResponses;

    #region MonoBehaviour Methods
    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (CollisionResponse collisionResponse in _collisionResponses)
        {
            if (other.transform.CompareTag(collisionResponse.Tag))
            {
                collisionResponse.OnCollisionEnterResponse?.Invoke();
            }
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        foreach (CollisionResponse collisionResponse in _collisionResponses)
        {
            if (other.transform.CompareTag(collisionResponse.Tag))
            {
                collisionResponse.OnCollisionStayResponse?.Invoke();
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        foreach (CollisionResponse collisionResponse in _collisionResponses)
        {
            if (other.transform.CompareTag(collisionResponse.Tag))
            {
                collisionResponse.OnCollisionExitResponse?.Invoke();
            }
        }
    }
    #endregion
}
