using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerController;
public interface IPlayerModule
{
    public RaycastPlayerController Controller { get; set; }

    //public PlayerInputHandler Input { get; set; }

    //when HealthPercentile reaches 0%, the module is "destroyed"
    public float HealthPercentile { get; }
    public void Damage(float subtractPercent);

    // public void ReadInput();
    // public void DoBehaviour();

}
