using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class seems to represent a dialogue tree that provides the capabilities
/// to utilize multiple segments of dialogue that can be displayed onto the screen
/// comments seem written well
/// </summary>
public class DialogueTree : ADialogueTree
{
  [SerializeField] private string[] segments;
  [SerializeField] private ADialogueTree next; // null represents that this is a leaf (end of tree)

  // Which segment is being written. Out-of-range index means the next node is being read.
  private int _idxOfSegment = -1;

  private int _segmentLength = 0; // How many characters of the current segment are written

  /// <summary>
  /// It seems that all this is supposed to do is that
  /// in the even that there is no dialogue left, it
  /// Will basically just signal that theres an issue.
  /// </summary>
  private void Awake()
  {
    if (segments.Length == 0)
    {
      Debug.LogError("No dialogues exist in this dialogue node. This does not make sense.");
    }
  }


  /// <summary>
  /// Advances the current text segment by one character. If the segment
  /// is complete (at full length), advances to next dialogue tree node.
  /// </summary> 
  public override void Advance()
  {
    if (InThisNode())
    {
      // If there is more to be advanced in the current segment
      if (_segmentLength < segments[_idxOfSegment].Length)
      {
        _segmentLength++;
      }
    }
    else
    {
      next.Advance();
    }
  }


  /// <summary>
  /// Goes straight to the end of the current text segment (puts it at
  /// full length).
  /// </summary>
  public override void AdvanceToEnd()
  {
    if (InThisNode())
    {
      _segmentLength = segments[_idxOfSegment].Length;
    }
    else
    {
      next.AdvanceToEnd();
    }
  }

  /// <summary>
  /// Gives the current text segment as it is so far. It may not be complete /
  /// at full length yet.
  /// </summary>
  public override string GetText()
  {
    return InThisNode() 
      ? segments[_idxOfSegment].Substring(0, _segmentLength) 
      : next.GetText();
  }


  /// <summary>
  /// Advances to the next segment, starting it at 0 characters in length. This
  /// method needs to be called to obtain the first segment. 
  /// </summary>
  public override void NextSegment()
  {
    print("Next segment");
    if (InThisNode())
    {
      _idxOfSegment++;
      _segmentLength = 0;
      
      // If this puts the segment index at the edge, go to index 0 in the next tree
      if (_idxOfSegment == segments.Length)
      {
        next.NextSegment();
      }
    }
    else
    {
      next.NextSegment();
    }
  }


  /// <summary>
  /// Is there a next segment?
  /// </summary>
  public override bool HasNextSegment()
  {
    return _idxOfSegment < segments.Length - 1 || next.HasNextSegment();
  }

  public override bool SegmentIsComplete()
  {
    if (InThisNode())
    {
      return _segmentLength == segments[_idxOfSegment].Length;
    }
    else
    {
      return next.SegmentIsComplete();
    }
  }


  /// <summary>
  /// As of now it appears this functionality isn't supported
  /// and now only debugs.
  /// </summary>
  /// <returns> nothing if no responses exist,
  /// otherwise return response array of a size one or greater </returns>
  public override string[] GetResponses()
  {
    Debug.LogError("Unimplemented Method");
    return null;
  }


  /// <summary>
  /// As of now it appears this functionality isn't supported
  /// and now only debugs.
  /// </summary>
  /// <param name="index"> segment index to retrieve response from </param>
  public override void ChooseResponse(int index)
  {
    Debug.LogError("Unimplemented Method");
    // Nothing
  }

  public override void Reset()
  {
    _idxOfSegment = -1;
    _segmentLength = 0;
    next.Reset();
  }

  // <summary>
  // Are this node's dialogues still being read?
  // </summary>

  /// <summary>
  /// Appears that this is a method offered by this class specifically
  /// to check if the dialogue still has stuff to read by checking if
  /// where you are reading is less that the length of your dialogue
  /// </summary>
  /// <returns> return false if the segment index is less than
  /// the amount of segments that exist </returns>
  private bool InThisNode()
  {
    return _idxOfSegment < segments.Length;
  }
}