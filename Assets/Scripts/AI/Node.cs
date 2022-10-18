using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract Node class. Has some boolean success value and a process to determine this node's 
// success.
public abstract class Node
    {
        protected bool _isSuccess;

        public bool GetSuccess()
        {
            return _isSuccess;
        }

        public abstract bool Process();
    }