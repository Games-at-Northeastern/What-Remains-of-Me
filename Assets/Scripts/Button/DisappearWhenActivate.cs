using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearWhenActivate : AObjectToActivate
{
    public override void Activate()
    {
        GetComponent<Renderer>().enabled = false;
        if (GetComponent<Collider2D>())
        {
            GetComponent<Collider2D>().gameObject.SetActive(false);
        }
    }

    public override void Deactivate()
    {
        GetComponent<Renderer>().enabled = true;
        if (GetComponent<Collider2D>())
        {
            GetComponent<Collider2D>().gameObject.SetActive(true);
        }
    }
}
