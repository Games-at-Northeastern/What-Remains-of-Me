using UnityEngine;

// <summary>
// The purpose of this script is to detach a child object from a parent object at runtime.
// The reason to use this is so that the child object can be a part of the prefab so that designers have less objects to drag into the scene, however the object won't move with the parent object
// One example of this is the reticle GameObject which is used for the auto targeting system.
// We need the reticle in the scene, however making it the child object of the player means that the reticle will move with the player
// We have the reticle as a child object of WireThrower so that it is dragged into the scene with the wirethrower, however the reticle detaches from the parent object so that the reticle does not move with the player
// </summary>

public class DetachFromParentOnStart : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);
    }

}
