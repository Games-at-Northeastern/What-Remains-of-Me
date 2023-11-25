using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A base abstract for scripts that change aspects of themselves using dynamic entries.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ADynamicAsset<T> : MonoBehaviour where T : IRangeVirusEntry
{
    [SerializeField]
    protected List<T> _assetBranches;

    protected virtual T MatchState(float playerVirus, float targetVirus)
    {
        foreach (var triple in _assetBranches)
        {
            if (triple.IsWithinRange(playerVirus, targetVirus))
            {
                return triple;
            }
        }

        throw new ArgumentException($"No valid dialogue branch for player virus {playerVirus} w/ target virus {targetVirus}.");
    }
}
