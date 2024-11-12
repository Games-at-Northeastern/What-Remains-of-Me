using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] public bool activate;
    [SerializeField] private Animator anim;
    void Start()
    {
        activate = false;
    }

    void Update()
    {

    }

    public void ColliderOff()
    {
        if (activate == false)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
            return;
    }
    public void ColliderOn()
    {

        GetComponent<BoxCollider2D>().enabled = true;
    }
}
