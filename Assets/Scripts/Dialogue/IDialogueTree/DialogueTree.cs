using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTree : ADialogueTree
{
    [SerializeField] string[] segments;
    [SerializeField] ADialogueTree next; // null represents that this is a leaf (end of tree)
    int idxOfSegment = -1; // Which segment is being written. Out-of-range index means the next node is being read.
    int segmentLength = 0; // How many characters of the current segment are written

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

    public override string[] GetResponses()
    {
        Debug.LogError("Unimplemented Method");
        return null;
    }

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

    /// <summary>
    /// Are this node's dialogues still being read?
    /// </summary>
    private bool inThisNode()
    {
        return idxOfSegment < segments.Length;
    }
}

