using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles making Vox appear more damaged visually after each puzzle.
/// Expects an animator for the overlay with an integer parameter named "DamageLevel".
/// </summary>
[RequireComponent(typeof(Animator))]
public class VoxDamageOverlayController : MonoBehaviour
{
    /// <summary>
    /// Represents a reference to one of the four puzzles, used to evaluate completion.
    /// </summary>
    [Serializable]
    public struct Puzzle
    {
        public AControllable controllable;
        [Range(0f, 1f)] public float expectedCharge;
    }
    
    [SerializeField] private List<Puzzle> puzzles;

    private Animator _anim;
    private int _prevCount;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Skip update if the damage level is already at maximum.
        if (_prevCount == puzzles.Count) return;
        
        // Filter the puzzle doors by opened state (which correspond to puzzle completion)
        var count = puzzles.Where(IsCompleted).ToList().Count;
        
        // If the new count is lower than highest seen, do not revert the damage state.
        if (count <= _prevCount) return;

        // Set the animator parameter
        _anim.SetInteger("DamageLevel", count);
        _prevCount = count;
    }

    /// <summary>
    /// Returns whether a puzzle is completed.
    /// </summary>
    /// <param name="puzzle">the target puzzle</param>
    /// <returns></returns>
    private static bool IsCompleted(Puzzle puzzle)
    {
        return Mathf.Approximately(puzzle.controllable.GetPercentFull(), puzzle.expectedCharge);
    }
}