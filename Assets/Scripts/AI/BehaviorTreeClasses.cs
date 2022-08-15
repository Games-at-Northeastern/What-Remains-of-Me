using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Namespace to organize BehaviorTree interface and classes.
namespace BehaviorTreeAI
{
    public interface BehaviorTree
    {
        // Initializes structure of behavior tree.
        public void InitializeBehaviorTree();

        // Runs the top-most node process.
        public void Run();

        // Runs function to use pathfinding data, if any
        public void Move(Vector3 target);

        // Function to be called upon being alerted by Surveillance Robot
        public void Alerted();
    }

    // Abstract Node class. Has some boolean success value and a process to determine this node's 
    // success.
    public abstract class Node
    {
        protected bool isSuccess;

        public bool GetSuccess()
        {
            return isSuccess;
        }

        public abstract bool Process();
    }

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

    // Inverter Node class. Runs source node. Returns true if child is false and vice versa.
    public class Inverter : Node
    {
        protected Node source;

        public Inverter(Node node)
        {
            source = node;
        }

        public override bool Process()
        {
            return !source.Process();
        }
    }
}