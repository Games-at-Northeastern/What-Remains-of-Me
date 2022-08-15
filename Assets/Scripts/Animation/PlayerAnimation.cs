using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private MovementExecuter movementExecuter;
    private WireThrower wireThrower;

    void Start()
    {
        animator = GetComponent<Animator>();
        movementExecuter = GetComponent<MovementExecuter>();
        wireThrower = GetComponent<WireThrower>();
    }

    void Update()
    {
        animator.SetInteger("StateID", (int) movementExecuter.GetCurrentMove().GetAnimationState());
        animator.SetBool("WireOut", wireThrower.WireExists());
    }
}
