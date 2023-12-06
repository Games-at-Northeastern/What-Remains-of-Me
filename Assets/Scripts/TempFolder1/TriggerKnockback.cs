using System.Collections;
using UnityEngine;

/// <summary>
/// A temporary component that exists to inflict knockback on the player after entering a trigger.
/// This is used for scene 3a's NPC event.
/// It is intentionally designed to be unusable in any other use case except for this one. This is because
/// the concept of "cutscene events" is a large undertaking and must be done as a separate task.
/// </summary>
public class TriggerKnockback : MonoBehaviour
{
    private Animator animator;
    private int _animStateHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (!animator)
        {
            throw new MissingReferenceException("Animator not found!");
        }

        _animStateHash = Animator.StringToHash("animState");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }

        StartCoroutine(IEKnockBack());

        var body = collision.attachedRigidbody;

        if (body)
        {
            body.AddForce(Quaternion.Euler(0f, 0f, -45f) * Vector3.left * 10f, ForceMode2D.Impulse);
        }
    }

    private IEnumerator IEKnockBack()
    {
        animator.SetInteger(_animStateHash, 2);

        yield return new WaitForSeconds(1f);

        animator.SetInteger(_animStateHash, 0);
    }
}
