using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Selector Node class. Runs list of children. Returns false if all children are false. Returns
// true if any child is true.
public class Selector : Node
{
    protected List<Node> _children;

    public Selector(List<Node> nodes)
    {
        _children = nodes;
    }

    public override bool Process()
    {
        foreach (Node child in _children)
        {
            if (child.Process())
            {
                _isSuccess = true;
                return true;
            }
        }

        _isSuccess = false;
        return false;
    }
}
