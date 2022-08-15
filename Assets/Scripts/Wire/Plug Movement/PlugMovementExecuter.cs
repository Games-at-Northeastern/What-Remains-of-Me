using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the movement of the wire, and whenever something happens that should
/// end the movement of the plug, sends an event communicating that.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlugMovementExecuter : MonoBehaviour
{
    public UnityEvent onTerminateRequest = new UnityEvent();
    public GameObjectEvent onConnectionRequest = new GameObjectEvent();
    IPlugMovementModel model = null;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Initiates the movement of this plug, using the given model to handle it.
    /// The model given to this function should be a newly created one.
    /// </summary>
    public void Fire(IPlugMovementModel model)
    {
        if (this.model == null)
        {
            this.model = model;
        }
    }

    private void Update()
    {
        if (model != null)
        {
            ApplyModel();
        }
    }

    /// <summary>
    /// Updates the movement model, moves the wire according to the model, and
    /// does anything else requested by the model.
    /// </summary>
    void ApplyModel()
    {
        model.AdvanceTime();
        rb.velocity = model.Velocity();
        if (model.Terminate())
        {
            onTerminateRequest.Invoke();
        }
    }

    /// <summary>
    /// For checking if this plug has come into contact with a plug. If this
    /// has happened, the move should end.
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.gameObject);
        if (collision.gameObject.layer == 8) // Outlet Collision
        {
            onConnectionRequest.Invoke(collision.gameObject);
        }
        else // Normal Collision
        {
            model.HandleCollision();
        }
    }
}
