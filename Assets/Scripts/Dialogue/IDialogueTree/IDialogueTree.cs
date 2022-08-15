using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for a dialogue tree, made up of individual dialogue segments
/// which show up one at a time. Allows dialogue to be animated, skipped,
/// and responded to.
/// </summary>
public interface IDialogueTree
{
    /// <summary>
    /// Advances the current text segment by one character. If the segment
    /// is complete (at full length), does nothing.
    /// </summary>
    public void Advance();

    /// <summary>
    /// Goes straight to the end of the current text segment (puts it at
    /// full length).
    /// </summary>
    public void AdvanceToEnd();

    /// <summary>
    /// Is the current text segment complete (at full length?)
    /// </summary>
    public bool SegmentIsComplete();

    /// <summary>
    /// Advances to the next segment, starting it at 0 characters in length. This
    /// method needs to be called to obtain the first segment. If there are
    /// no more segments in the tree, nothing will happen.
    /// </summary>
    public void NextSegment();

    /// <summary>
    /// Is there a next segment?
    /// </summary>
    public bool HasNextSegment();

    /// <summary>
    /// Gives the current text segment as it is so far. It may not be complete /
    /// at full length yet.
    /// </summary>
    public string GetText();

    /// <summary>
    /// Returns all the possible responses to the current text segment. If
    /// there are no responses, returns null. If there are responses, the
    /// returned array will be of at least length 1.
    /// </summary>
    public string[] GetResponses();

    /// <summary>
    /// Select the response of the given index. This will cause the tree to advance
    /// to the appropriate next text segment (as in, you don't need to call
    /// NextSegment() to get it). If there are no responses, does nothing.
    /// </summary>
    public void ChooseResponse(int index);

    /// <summary>
    /// Reset the internal state of the dialogue tree so that it can be animated
    /// all over again like new.
    /// </summary>
    public void Reset();
}
