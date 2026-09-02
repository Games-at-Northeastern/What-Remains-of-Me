using UnityEngine;
using PlayerController;
using System;
//using System.Threading.Tasks;


public class DialogueCamera : MonoBehaviour
{
    private PlayerController2D characterController;

    private Unity.Cinemachine.CinemachineCamera dialogueCameara;
    private Unity.Cinemachine.CinemachineTargetGroup targetGroup;


    float prevPlayerWeight, prevPlayerRadius;
    float prevTargetWeight, prevTargetRadius;
    public static DialogueCamera Instance { get; private set; }

    void Start()
    {
        Instance = this;

        dialogueCameara = GetComponentInChildren<Unity.Cinemachine.CinemachineCamera>();
        characterController = FindAnyObjectByType<PlayerController2D>();
        targetGroup = GetComponentInChildren<Unity.Cinemachine.CinemachineTargetGroup>();

        var playerTarget = new Unity.Cinemachine.CinemachineTargetGroup.Target
        {
            Object = characterController.transform,
            Weight = 1f,
            Radius = 0f
        };
        targetGroup.AddMember(characterController.transform, 1f, 0f);

        targetGroup.Targets[0] = playerTarget;

        StopFramingDialogue();
    }
    public void StartFramingDialogue(Transform speaker)
    {
        prevPlayerWeight = targetGroup.Targets[0].Weight;
        prevPlayerRadius = targetGroup.Targets[0].Radius;


        targetGroup.Targets[0].Weight = 0f;
        targetGroup.Targets[0].Radius = 0f;

        prevTargetWeight = targetGroup.Targets[1].Weight;
        prevTargetRadius = targetGroup.Targets[1].Radius;

        targetGroup.Targets[1] = new Unity.Cinemachine.CinemachineTargetGroup.Target
        {
            Object = speaker,
            Weight = 1.2f,
            Radius = 0.1f
        };

        dialogueCameara.Priority = 100;
    }

    public void StopFramingDialogue()
    {
        targetGroup.Targets[0] = new Unity.Cinemachine.CinemachineTargetGroup.Target
        {
            Object = targetGroup.Targets[0].Object,
            Weight = prevPlayerWeight == 0 ? 1f : prevPlayerWeight,
            Radius = prevPlayerRadius
        };

        targetGroup.Targets[1] = new Unity.Cinemachine.CinemachineTargetGroup.Target
        {
            Object = targetGroup.Targets[1].Object,
            Weight = 0f,
            Radius = prevTargetRadius
        };

        dialogueCameara.Priority = -100;
    }
}
