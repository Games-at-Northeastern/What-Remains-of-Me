using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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