using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : Interaction
{
   public MovingElement movingElement;

   public override void Execute()
   {
       movingElement.Activate();
   }
}
