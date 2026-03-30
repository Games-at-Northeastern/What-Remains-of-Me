using System.Collections.Generic;
using System.Linq;
using PlayerController;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Controls the cutscene playback when the first outlet is unlocked to the player.
/// </summary>
public class VoxFirstOutletCutsceneTrigger : MonoBehaviour
{
    [Header("Cutscene")]
    [SerializeField] private PlayableDirector cutscene;
    
    [Header("Condition")]
    [SerializeField] private List<AControllable> doors;

    private PlayerController2D _player;
    
    private bool _hasPlayed;

    private void Start()
    {
        cutscene.playOnAwake = false;
        cutscene.stopped += OnCutsceneStopped;
        _player = FindFirstObjectByType<PlayerController2D>();
    }

    private void Update()
    {
        if (!_hasPlayed && ShouldPlay()) PlayCutscene();
    }
    
    /// <summary>
    /// Returns whether the cutscene should play.
    /// </summary>
    /// <returns></returns>
    private bool ShouldPlay()
    {
        return doors.All(d => Mathf.Approximately(d.GetPercentFull(), 0f));
    }

    /// <summary>
    /// Play the cutscene and lock player control.
    /// </summary>
    private void PlayCutscene()
    {
        _hasPlayed = true;
        _player.LockInputs();
        cutscene.Play();
    }

    /// <summary>
    /// Restore control and disable gameobject when the cutscene is stopped.
    /// </summary>
    /// <param name="_"></param>
    private void OnCutsceneStopped(PlayableDirector _)
    {
        _player.UnlockInputs();
        gameObject.SetActive(false);
    }
}