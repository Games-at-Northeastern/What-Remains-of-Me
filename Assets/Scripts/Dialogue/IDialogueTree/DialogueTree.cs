using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class seems to represent a dialgue tree that provides the capabilities
/// to utilize multiple segements of dialogue that can be displayed onto the screen
/// comments seem written well
/// </summary>
public class DialogueTree : ADialogueTree
{
    [SerializeField] string[] segments;
    [SerializeField] ADialogueTree next; // null represents that this is a leaf (end of tree)
    int idxOfSegment = -1; // Which segment is being written. Out-of-range index means the next node is being read.
    int segmentLength = 0; // How many characters of the current segment are written

    /// <summary>
    /// It seems that all this is supposed to do is that
    /// in the even that there is no dialogue left, it
    /// Will basically just signal that theres an issue.
    /// </summary>
    void Awake()
    {
        if (segments.Length == 0)
        {
            Debug.LogError("No dialogues exist in this dialogue node. This does not make sense.");
        }
    }

    public override void Advance()
    {
        if (inThisNode())
        {
            if (segmentLength < segments[idxOfSegment].Length) // If there is more to be advanced in the current segment
            {
                segmentLength++;
            }
        }
        else
        {
            next.Advance();
        }
    }

    public override void AdvanceToEnd()
    {
        if (inThisNode())
        {
            segmentLength = segments[idxOfSegment].Length;
        }
        else
        {
            next.AdvanceToEnd();
        }
    }

    public override string GetText()
    {
        if (inThisNode())
        {
            return segments[idxOfSegment].Substring(0, segmentLength);
        }
        else
        {
            return next.GetText();
        }
    }

    public override void NextSegment()
    {
        print("Next segment");
        if (inThisNode())
        {
            idxOfSegment++;
            segmentLength = 0;
            if (idxOfSegment == segments.Length) // If this puts the segment index at the edge, go to index 0 in the next tree
            {
                next.NextSegment();
            }
        }
        else
        {
            next.NextSegment();
        }
    }

    public override bool HasNextSegment()
    {
        return (idxOfSegment < segments.Length - 1) || next.HasNextSegment();
    }

    public override bool SegmentIsComplete()
    {
        if (inThisNode())
        {
            return segmentLength == segments[idxOfSegment].Length;
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
    /// <param name="index"> segment index to retrive response from </param>
    public override void ChooseResponse(int index)
    {
        Debug.LogError("Unimplemented Method");
        // Nothing
    }

    public override void Reset()
    {
        idxOfSegment = -1;
        segmentLength = 0;
        next.Reset();
    }

    // <summary>
    // Are this node's dialogues still being read?
    // </summary>

    /// <summary>
    /// Appears that this is a method offered by this class specifically
    /// to check if the dialouge still has stuff to read by checking if
    /// where you are reading is less that the length of your dialouge
    /// </summary>
    /// <returns> return false if the segment index is less than
    /// the amount of segments that exist </returns>
    private bool inThisNode()
    {
        return idxOfSegment < segments.Length;
    }
}

