using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    private VisualEffect virusEffect;

    [SerializeField]
    private VisualEffect virusEffect2;

    [SerializeField]
    private float doorMoveSpeed;

    private Vector2 initPos;

    private AudioSource openingAudioLoop;

    private Vector2 currentPos;

    private float initDensity;

    private void Awake()
    {
        initPos = transform.position;

        openingAudioLoop = GetComponent<AudioSource>();

        currentPos = transform.position;

        initDensity = virusEffect.GetFloat("Density");
    }

    /// <summary>
    /// Updates the door's position based on the amount of energy supplied to it.
    /// </summary>
    void Update()
    {
        float? virusPercent = GetVirusPercent();
        float totalEnergy = GetEnergy() + GetVirus();
        //if (virusPercent > doVirusEffectAt * 10f)
        //{
        //    virusEffect.enabled = true;
        //} else
        //{
        //    virusEffect.enabled = false;
        //}
        //Debug.Log("virus percent: " + virusPercent);
        if (totalEnergy < 1)
        {
            virusEffect.SetFloat("Density", 0f);
            virusEffect2.SetFloat("Density", 0f);
        } else
        {
            virusEffect.SetFloat("Density", initDensity);
            virusEffect2.SetFloat("Density", initDensity);
        }
        if (virusPercent != null) {
            if (virusPercent < doVirusEffectAt) {
                virusEffect.SetFloat("Density", 0f);
                virusEffect2.SetFloat("Density", 0f);
                Vector2 targetPos = Vector2.Lerp(initPos, initPos + posChangeForMaxEnergy, this.GetPercentFull());
                //Debug.Log("target pos: " + targetPos);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, doorMoveSpeed * Time.deltaTime);

                if (currentPos.y != transform.position.y)
                {
                    if (openingAudioLoop != null && !openingAudioLoop.isPlaying)
                    {
                        openingAudioLoop.Play(); //play sound if moving (sound will loop automatically)
                    }
                }
                else if (openingAudioLoop != null)
                {
                    openingAudioLoop.Stop(); //stop sound if not moving
                }

                currentPos = transform.position;

            } 
            else
            {
                //virusEffect.SetFloat("Density", virusPercent.Value);
            }
        } else {
            //virusEffect.SetFloat("Density", 0f);
        }
    }
}
