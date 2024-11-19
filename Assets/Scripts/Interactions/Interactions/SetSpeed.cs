using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpeed : Interaction
{
   public MovingElement movingElement;
   public float speed = 0f;

   public void execute()
   {
       movementElement.setSpeedModifier(speed);
   }


}
