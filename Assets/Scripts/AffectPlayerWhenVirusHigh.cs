using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AffectPlayerWhenVirusHigh : MonoBehaviour
{
    [SerializeField] private PlayerInfo AtlasPlayerInfo;

    [SerializeField] private MovementSettings DefaultMovementSettings;

    [SerializeField] private Volume v;

    private Vignette vg;
    private float originalRunMaxSpeed;
    private void Start()
    {
        originalRunMaxSpeed = DefaultMovementSettings.runMaxSpeed;

        v.profile.TryGet(out vg);
    }


    private void Update()
    {
        //Debug.Log(AtlasPlayerInfo.virus);
        //Debug.Log(AtlasPlayerInfo.maxVirus * 0.8f);
        /*if (AtlasPlayerInfo.virus > AtlasPlayerInfo.maxVirus * 0.5f)
        {
            DefaultMovementSettings.runMaxSpeed = 0.5f;
        }
        else
        {
            DefaultMovementSettings.runMaxSpeed = originalRunMaxSpeed;
        }
*/

        vg.intensity.value = AtlasPlayerInfo.virus / AtlasPlayerInfo.battery;

    }

    private void OnApplicationQuit()
    {
        DefaultMovementSettings.runMaxSpeed = originalRunMaxSpeed;
    }
}

