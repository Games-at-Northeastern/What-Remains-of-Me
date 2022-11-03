using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sequence Node class. Runs list of children. Returns false if any child is false. Returns 
// true if all children are true.
public class Sequence : Node
    {
        protected List<Node> _children;

        public Sequence(List<Node> nodes)
        {
            _children = nodes;
        }

        public override bool Process()
        {
            foreach (Node child in _children)
            {
                if (!child.Process())
                {
                    _isSuccess = false;
                    return false;
                }
            }

            _isSuccess = true;
            return true;
        }
    }
