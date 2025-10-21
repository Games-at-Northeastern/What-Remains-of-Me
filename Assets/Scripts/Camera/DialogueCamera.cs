using UnityEngine;
using PlayerController;
using Cinemachine;

public class DialogueCamera : MonoBehaviour
{
    private PlayerController2D characterController;

    private CinemachineVirtualCamera dialogueCameara;
    private CinemachineTargetGroup targetGroup;


    float prevPlayerWeight, prevPlayerRadius;
    float prevTargetWeight, prevTargetRadius;
    public static DialogueCamera Instance { get; private set; }

    void Start()
    {
        Instance = this;

        dialogueCameara = GetComponentInChildren<CinemachineVirtualCamera>();
        characterController = FindFirstObjectByType<PlayerController2D>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();

        var playerTarget = new CinemachineTargetGroup.Target
        {
            target = characterController.transform,
            weight = 1f,
            radius = 0f
        };

        targetGroup.m_Targets[0] = playerTarget;

        StopFramingDialogue();
    }
    public void StartFramingDialogue(Transform speaker)
    {
        prevPlayerWeight = targetGroup.m_Targets[0].weight;
        prevPlayerRadius = targetGroup.m_Targets[0].radius;


        targetGroup.m_Targets[0].weight = 0f;
        targetGroup.m_Targets[0].radius = 0f;

        prevTargetWeight = targetGroup.m_Targets[1].weight;
        prevTargetRadius = targetGroup.m_Targets[1].radius;

        targetGroup.m_Targets[1] = new CinemachineTargetGroup.Target
        {
            target = speaker,
            weight = 1.2f,
            radius = 0.1f
        };

        dialogueCameara.Priority = 100;
    }

    public void StopFramingDialogue()
    {
        targetGroup.m_Targets[0] = new CinemachineTargetGroup.Target
        {
            target = targetGroup.m_Targets[0].target,
            weight = prevPlayerWeight == 0 ? 1f : prevPlayerWeight,
            radius = prevPlayerRadius
        };

        targetGroup.m_Targets[1] = new CinemachineTargetGroup.Target
        {
            target = targetGroup.m_Targets[1].target,
            weight = 0f,
            radius = prevTargetRadius
        };

        dialogueCameara.Priority = -100;
    }
}
