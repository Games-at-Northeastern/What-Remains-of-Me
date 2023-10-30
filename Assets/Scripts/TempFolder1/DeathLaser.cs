using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DeathLaser : MonoBehaviour
{
    [SerializeField, Tooltip("Draws the laser in the direction of this target.")]
    private Transform _laserTarget;

    [SerializeField]
    private SpikeTeleport _deathTeleporter; // TODO, why do I have to use the SpikeTeleport.cs? Please refactor.

    [SerializeField]
    private bool _drawToTarget = true;

    [SerializeField]
    private bool _needsRecalculation = false;

    [SerializeField, Tooltip("How long is the laser? Leave at -1 for auto-distance")]
    private float _laserDistance = -1;

    [SerializeField]
    private LayerMask _mask;

    [SerializeField]
    private string _tag = "Player";

    private LineRenderer _renderer;
    private Vector3[] _points; // invariant: this array has 2 elements.
    private Vector3 _dir;

    private void Awake()
    {
        _renderer = GetComponent<LineRenderer>();
        Recalculate();
    }

    private void Update()
    {
        if (_needsRecalculation)
        {
            Recalculate();
        }

        CheckForDeath();
    }

    private void Recalculate()
    {
        _dir = (_laserTarget.position - transform.position).normalized;
        Vector3 targetPos;

        // draw laser to target point
        if (_drawToTarget)
        {
            targetPos = _laserTarget.position;
        }
        // draw laser in direction of target point until we hit something
        else if (_laserDistance <= 0 && DoRaycast(out RaycastHit2D data))
        {
            targetPos = data.point;
        }
        // draw laser in direction of target for a certain distance.
        else
        {
            targetPos = transform.position + (_dir * _laserDistance);
        }

        _points = new Vector3[] { transform.position, targetPos };

        _renderer.SetPositions(_points);
    }

    private bool DoRaycast(out RaycastHit2D data)
    {
        data = Physics2D.Raycast(transform.position, _dir, Mathf.Infinity, _mask);

        return data.point != Vector2.zero;
    }

    private void CheckForDeath()
    {
        if (DoRaycast(out RaycastHit2D data) && data.collider != null && data.collider.CompareTag(_tag))
        {
            _deathTeleporter.PerformDeath(data.collider.gameObject);
        }
    }
}
