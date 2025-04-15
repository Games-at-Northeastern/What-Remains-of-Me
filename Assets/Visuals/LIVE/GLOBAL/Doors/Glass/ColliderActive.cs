using UnityEngine;

public class ColliderActive : MonoBehaviour
{
    // kinematic rigidbodies only interact with dyanmic, this is to make it so it does it with static and kinematic as well
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.useFullKinematicContacts = true;
    }

}
