using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class helps with the transitions between animation states. 
 * It has 3 private variables: animator, the animator object, 
 * movementExecuter, a reference to the movement script so we can better 
 * understand the player's location and what animations will be necessary for it, 
 * and wireThrower, which gives the animator reference to where the wire is as it 
 * is important to the animation of the player.
 */
public class PlayerAnimation : MonoBehaviour
{
    //the animator object, a built-in unity interface that helps us execeute animations
    private Animator animator;
    //a reference to the movement script, to better understand the location of the player
    private MovementExecuter movementExecuter;
    //a reference to the wire, to better understand the location of the wire
    private WireThrower wireThrower;

    /*
     * When the game starts the private varaibles (animator, movmentExecutor, and wireThrower) are initialized.
     */
    void Start()
    {
        animator = GetComponent<Animator>();
        movementExecuter = GetComponent<MovementExecuter>();
        wireThrower = GetComponent<WireThrower>();
    }

    /*
     * Every frame this method is called. It updates the integer according to the movementExecutor object and 
     * the numbers represent certain animations, these can be found in file: Animation type. It also sets a boolean 
     * value for if the wire is currently in use/thrown.
     */
    void Update()
    {
        animator.SetInteger("StateID", (int) movementExecuter.GetCurrentMove().GetAnimationState());
        animator.SetBool("WireOut", wireThrower.WireExists());
    }
}
