using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inverter Node class. Runs source node. Returns true if child is false and vice versa.
public class Inverter : Node
{
    protected Node _source;

    public Inverter(Node node)
    {
        _source = node;
    }

    public override bool Process()
    {
        return !_source.Process();
    }
}

