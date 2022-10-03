using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sequence Node class. Runs list of children. Returns false if any child is false. Returns 
// true if all children are true.
public class Sequence : Node
    {
        protected List<Node> children;

        public Sequence(List<Node> nodes)
        {
            children = nodes;
        }

        public override bool Process()
        {
            foreach (Node child in children)
            {
                if (!child.Process())
                {
                    isSuccess = false;
                    return false;
                }
            }

            isSuccess = true;
            return true;
        }
    }
