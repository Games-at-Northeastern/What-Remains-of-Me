using System;
using System.Collections;
using System.Collections.Generic;
using PlayerController;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// A customizable sequence player intended for Vox's ability to control platforms to interrupt the player.
/// However, it can also be used for other scenarios.
/// <para></para>
/// Customizable options:
/// <list type="bullet">
///     <item>Trigger Condition: Collider or specific controllable energy level</item>
///     <item>Cutscene: Cutscene to play when the condition is met</item>
///     <item>Dialogue: Dialogue to play when the condition is met</item>
///     <item>
///         Platforms: Each platform path object can either be stopped immediately or after
///         reaching a specific location.
///     </item>
/// </list>
/// </summary>
public class PlatformSequencePlayer : MonoBehaviour
{
    /// <summary>
    /// The type of trigger for when the sequence should play.
    /// Controllable: Triggers when the reference controllable is at a specific energy level.
    /// Collider:     Triggers when the player enters the reference collider.
    /// </summary>
    private enum TriggerType
    {
        Controllable, Collider
    }

    /// <summary>
    /// Represent the configuration of a specific platform.
    /// </summary>
    [Serializable]
    private struct PlatformConfig
    {
        public MovingElementPathScript platformPath;
        public bool moveToLocation;
        [Range(0f, 1f)] public float location;
    }

    [Header("Trigger")] 
    [SerializeField] private bool playOnce = true;
    [SerializeField] private bool triggerEnabled;
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private AControllable controllable;
    [SerializeField] [Range(0f, 1f)] private float targetEnergy;

    [Header("Cutscene")] 
    [SerializeField] private bool cutsceneEnabled;
    [SerializeField] private PlayableDirector cutscene;

    [Header("Dialogue")] 
    [SerializeField] private bool dialogueEnabled;
    [SerializeField] private InkDialogueTrigger dialogue;

    [Header("Platforms")]
    [SerializeField] private List<PlatformConfig> platforms = new();

    private PlayerController2D _player;
    private bool _playerInRange;
    
    private bool _hasPlayed;
    private Coroutine _sequence;

    private void Start()
    {
        _player = FindFirstObjectByType<PlayerController2D>();
    }

    private void Update()
    {
        if (!ShouldPlay()) return;
        
        if (_sequence != null) return;
        _sequence = StartCoroutine(PlaySequence());
    }

    /// <summary>
    /// Returns whether the condition for playing is met.
    /// </summary>
    /// <returns></returns>
    private bool ShouldPlay()
    {
        if (_hasPlayed && playOnce) return false;
        if (!triggerEnabled) return false;
        
        return triggerType switch
        {
            TriggerType.Controllable => Mathf.Approximately(controllable.GetPercentFull(), targetEnergy),
            TriggerType.Collider => _playerInRange,
            _ => false,
        };
    }

    /// <summary>
    /// Coroutine for the sequence.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlaySequence()
    {
        _hasPlayed = true;
        
        // Play cutscene
        if (cutsceneEnabled)
        {
            _player.LockInputs();
            cutscene.Play();
            cutscene.stopped += _ =>
            {
                _player.UnlockInputs();
                cutscene.enabled = false;
            };
        }

        // Start dialogue
        if (dialogueEnabled)
        {
            dialogue.StartDialogue();
        }
        
        // Move platforms
        foreach (var platform in platforms)
        {
            if (platform.platformPath == null) continue;
            MovePlatform(platform);
        }

        _sequence = null;
        yield break;
    }

    /// <summary>
    /// Move the platform. Caller is responsible for null-checking.
    /// </summary>
    /// <param name="cfg">configuration for the platform</param>
    private static void MovePlatform(PlatformConfig cfg)
    {
        if (cfg.moveToLocation)
        {
            cfg.platformPath.StopAt(cfg.location);
        }
        else
        {
            cfg.platformPath.Deactivate();
        }
    }

    /// <summary>
    /// Play this sequence if the sequence has not already started.
    /// </summary>
    public void Play()
    {
        if (_sequence != null) return;
        _sequence = StartCoroutine(PlaySequence());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) _playerInRange = false;
    }

    private void OnValidate()
    {
        // Configure the trigger collider and add one if missing.
        if (triggerEnabled && triggerType == TriggerType.Collider)
        {
            if (TryGetComponent<Collider2D>(out var col))
            {
                col.isTrigger = true;
            }
            else
            {
                col = gameObject.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }
        }
    }
}