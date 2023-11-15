using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotDialogue : MonoBehaviour
{
    [SerializeField]
    private List<DialogueTriple> _dialogueBranches;
    [SerializeField]
    private PlayerInfo _playerInfo;

    [Space(15)]

    [SerializeField, Tooltip("The player's transform. If left null, assigned to the transform of the Player-tagged object.")]
    private Transform _player;
    [SerializeField, Tooltip("The NPC's virus source. If left null, assigned to the one on this GameObject.")]
    private AControllable _paired;

    private InkDialogueManager _cachedInstance;

    private void Awake()
    {
        _cachedInstance = InkDialogueManager.GetInstance();

        try
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        catch (NullReferenceException e)
        {
            throw new MissingReferenceException("Player-tagged GameObject is missing from scene!", e);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MatchState(_playerInfo.virus, _paired.GetVirus());
        }
    }

    private DialogueTriple MatchState(float playerVirus, float targetVirus)
    {
        foreach (var triple in _dialogueBranches)
        {
            if (triple.IsWithinRange(playerVirus, targetVirus))
            {
                return triple;
            }
        }

        throw new ArgumentException($"No valid dialogue branch for player virus {playerVirus} w/ target virus {targetVirus}.");
    }
}
