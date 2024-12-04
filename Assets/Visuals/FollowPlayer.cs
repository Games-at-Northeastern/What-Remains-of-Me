using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject player;
    private float distToPlayer;
    private float lerpSpeed = .5f;
    private float maxDistance = 2f;
    private float distFromParent;
    private Vector3 parentPosition;
    void Start()
    {
        parentPosition = parent.transform.position;
    }

    void Update()
    {
        parentPosition = parent.transform.position;
        distFromParent = Mathf.Sqrt((transform.position - parentPosition).sqrMagnitude);
        Vector3 targetPosition = Vector3.Lerp(transform.position, player.transform.position, lerpSpeed);
        targetPosition = Vector3.MoveTowards(parentPosition, targetPosition, Mathf.Min(Vector3.Distance(transform.position, targetPosition), maxDistance));
        transform.position = targetPosition;
    }
}
