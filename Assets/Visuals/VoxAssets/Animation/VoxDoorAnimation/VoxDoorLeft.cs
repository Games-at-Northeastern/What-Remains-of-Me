using UnityEngine;

public class VoxDoorLeft : MonoBehaviour
{
    [SerializeField] private Animator otherDoorAnimator;
    private Animator selfAnimator;

    // getting self's animator
    void Start()
    {
        selfAnimator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if(otherDoorAnimator.GetBool("Opening"))
        {
            selfAnimator.SetBool("Left", true);
            selfAnimator.SetBool("Opening", true);
        }
    }
}
