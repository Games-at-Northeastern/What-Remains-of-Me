using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AnimationCutscene : MonoBehaviour
{
    // the animator you want to use for this function. must contain two parameters: doAnimate, and didAnimate, which are hooked between
    // a starting state, the state or states that play during the cutscene, and a "finished" state, which is hooked to the end state.
    // the beginning should then loop back to the end only if the didAnimate parameter is true.
    public Animator animator;

    // this virtual camera is panned to. Íf you don't want to use this, set it to be the same as the player camera.
    public CinemachineVirtualCamera panCam;
    // put the default virtual cam for the player here.
    public CinemachineVirtualCamera playerCam;
    // put the player's rigidbody here.
    public Rigidbody2D playerBody;
    private bool doCutscene;
    private bool moveCamera;
    [SerializeField] float waitTime = 1; // the amount of time to wait between moving the camera and starting the animation. Set to zero if you aren't moving the camera.
    // Start is called before the first frame update
    void Start()
    {
        doCutscene = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveCamera)
        {
            playerBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            if (panCam != playerCam)
                panCam.Priority = playerCam.Priority + 1;
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                moveCamera = false;
                doCutscene = true;
            }
        }

        if (doCutscene)
        {
            animator.SetBool("doAnimate", true);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
            {
                if (panCam != playerCam)
                    panCam.Priority = playerCam.Priority - 1;
                doCutscene = false;
                playerBody.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
                animator.SetBool("didAnimate", true);
                Destroy(gameObject);
            }
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        moveCamera = true;
    }
}
