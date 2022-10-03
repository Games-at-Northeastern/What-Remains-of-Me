using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Selector Node class. Runs list of children. Returns false if all children are false. Returns
// true if any child is true.
public class Selector : Node
{
    protected List<Node> children;

    public Selector(List<Node> nodes)
    {
        children = nodes;
    }

    public override bool Process()
    {
        foreach (Node child in children)
        {
            if (child.Process())
            {
                isSuccess = true;
                return true;
            }
        }

        isSuccess = false;
        return false;
    }
}
