using UnityEngine;
using PlayerController;
using Cinemachine;

public class DialogueCamera : MonoBehaviour
{
    private PlayerController2D characterController;

    private CinemachineVirtualCamera dialogueCameara;
    private CinemachineTargetGroup targetGroup;

    public static DialogueCamera Instance { get; private set; }

    private void Start()
    {
        Instance = this;

        dialogueCameara = GetComponentInChildren<CinemachineVirtualCamera>();
        characterController = FindFirstObjectByType<PlayerController2D>();
        targetGroup = GetComponentInChildren<CinemachineTargetGroup>();


        var playerTarget = new CinemachineTargetGroup.Target();
        playerTarget.target = characterController.transform;
        playerTarget.weight = 1;

        targetGroup.m_Targets[0] = playerTarget;

        //Just incase our camera priority starts too high
        StopFramingDialogue();
    }

    public void StartFramingDialogue(Transform speaker)
    {
        //We add the speaker into the tracking group and let cinemachine deal with the rest
        var speakerTarget = new CinemachineTargetGroup.Target();
        speakerTarget.target = speaker.transform;
        speakerTarget.weight = 1;

        targetGroup.m_Targets[1] = speakerTarget;

        dialogueCameara.Priority = 100;
    }

    public void StopFramingDialogue() => dialogueCameara.Priority = -100;
}
