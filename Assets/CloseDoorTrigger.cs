using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorTrigger : Interaction
{
    public Outlet outlet;

	public override void Execute()
	{
		outlet.EmptyEnergy();
	}


}
