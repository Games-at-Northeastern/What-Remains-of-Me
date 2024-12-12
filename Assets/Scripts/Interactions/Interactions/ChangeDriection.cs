using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDriection : Interaction
{
    public MovingElement movingElement;
    public override void Execute()
    {
        movingElement.ToggleDir();
    }

}
