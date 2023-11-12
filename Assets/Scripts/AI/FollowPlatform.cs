using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlatform : MonoBehaviour
{
    [SerializeField] private Transform outlet;
    [SerializeField] private Transform platform;

    private Vector3 offset;

    private void Start()
    {
        offset = new Vector3(platform.position.x - outlet.position.x, -Mathf.Abs(platform.position.y - outlet.position.y), 0f);
    }

    // Update is called once per frame
    void Update()
    {
        outlet.position = platform.position + offset;
    }
}
