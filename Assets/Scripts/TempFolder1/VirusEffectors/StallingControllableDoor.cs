using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a door which can be moved up or down based on the amount of energy
/// supplied to it, but that stalls in its current position whenever its virus percentage
/// gets above a certain threshold.
/// </summary>
public class StallingControllableDoor : AControllable
{
    [SerializeField] 
    private Vector2 posChangeForMaxEnergy;
    
    [SerializeField] 
    private float doVirusEffectAt;

    [SerializeField]
    private float doorMoveSpeed;

    private Vector2 initPos;

    private void Awake()
    {
        initPos = transform.position;
    }

    /// <summary>
    /// Updates the door's position based on the amount of energy supplied to it.
    /// </summary>
    void Update()
    {
        float? virusPercent = GetVirusPercent();
        Debug.Log("virus percent: " + virusPercent);
        if (virusPercent != null && virusPercent < doVirusEffectAt) {
            Vector2 targetPos = Vector2.Lerp(initPos, initPos + posChangeForMaxEnergy, this.GetPercentFull());
            Debug.Log("target pos: " + targetPos);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, doorMoveSpeed * Time.deltaTime);
        }
    }
}
