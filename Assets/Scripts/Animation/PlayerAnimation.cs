using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterController;
/// <summary>
///  This class helps with the transitions between animation states. 
///  It has 3 private variables: animator, the animator object,
///  movementExecuter, a reference to the movement script so we can better 
///  understand the player's location and what animations will be necessary for it, 
///  and wireThrower, which gives the animator reference to where the wire is as it 
///  is important to the animation of the player.
/// </summary>
/// 
public class PlayerAnimation : MonoBehaviour
{
    /// <summary>
    /// the animator object, a built-in unity interface that helps us execeute animations
    /// </summary>
    [SerializeField] private Animator _animator;
    /// <summary>
    /// a reference to the movement script, to better understand the location of the player
    /// </summary>
    [SerializeField] private CharacterController2D cc;
    private void Start()
    {
        cc = GetComponentInParent<CharacterController2D>();
    }
    /// <summary>
    /// a reference to the wire, to better understand the location of the wire
    /// </summary>
    [SerializeField] private WireThrower _wireThrower;

    /// <summary>
    /// Every frame this method is called. It updates the integer according to the movementExecutor object and 
    /// the numbers represent certain animations, these can be found in file: Animation type. It also sets a boolean 
    /// value for if the wire is currently in use/thrown.
    /// </summary>
    void Update()
    {
        _animator.SetInteger("StateID", (int) cc.GetAnimationState());
        _animator.SetBool("WireOut", _wireThrower.WireExists());
    }
}
