using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintHelloInteraction : Interaction
{

    public override void Execute() => Debug.Log("Hello");
}
