using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
