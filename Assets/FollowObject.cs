using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject target;
    // offset can be added to this object so it can follow target GameObject without colliding
    public float offsetX;
    public float offsetY;

    void Update()
    {
        this.transform.position = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, this.transform.position.z);
    }
}
