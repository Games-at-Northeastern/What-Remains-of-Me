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
        if (activate == true)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }

    }

    public void ColliderOff()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        activate = false;
    }
    public void ColliderOn()
    {
        GetComponent<BoxCollider2D>().enabled = true;
        activate = true;
    }
}
