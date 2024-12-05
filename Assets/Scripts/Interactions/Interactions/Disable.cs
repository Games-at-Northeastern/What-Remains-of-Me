using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : Interaction
{
   public MonoBehaviour behavior;
   public LineRenderer renderer;

    public override void Execute()
    {
        behavior.enabled = false;
        renderer.enabled = false;
    }
}
