using UnityEngine;

public class FollowParallax : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Header("Target and follower (this) are aligned at first positions:")]
    [SerializeField] private Vector3 targetPosition1;
    [SerializeField] private Vector3 followerPosition1;
    [Header("Target and follower (this) are aligned at second positions:")]
    [SerializeField] private Vector3 targetPosition2;
    [SerializeField] private Vector3 followerPosition2;
    private Vector3 followerRate = Vector3.zero;

    private enum FollowTarget { PLAYER, CAMERA }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target is null: follow parallax will fail");
            return;
        }
        CalculateFollowerRate();
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }
        var targetDelta = target.position - targetPosition1;
        gameObject.transform.position = new Vector3(
            followerPosition1.x + (followerRate.x * targetDelta.x),
            followerPosition1.y + (followerRate.y * targetDelta.y),
            1);
    }

    private void CalculateFollowerRate()
    {
        var targetDistance = targetPosition2 - targetPosition1;
        var followerDistance = followerPosition2 - followerPosition1;
        followerRate = new Vector3(followerDistance.x / targetDistance.x, followerDistance.y / targetDistance.y, 1);
    }

    // player rate: 1, wall rate: 0.0301, aligned if player (-9.375, 190.5), aligned if wall (-0.3 => 5.72)
    // [SerializeField] private float playerRate;
    // [SerializeField] private float wallRate;
    // [SerializeField] private float alignedIfPlayerAtX;
    // [SerializeField] private float alignedIfWallAtX;
    // private Transform player;

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // private void Start()
    // {
    //     player = GameObject.FindGameObjectWithTag("Player").transform;
    // }

    // // Update is called once per frame
    // private void Update()
    // {
    //     if (player == null)
    //     {
    //         return;
    //     }

    //     float playerDeltaFromAlignment = player.position.x - alignedIfPlayerAtX;
    //     float wallDeltaFromAlignment = playerDeltaFromAlignment * (wallRate / playerRate);
    //     gameObject.transform.position = new Vector3(alignedIfWallAtX + wallDeltaFromAlignment, gameObject.transform.position.y, gameObject.transform.position.z);
    // }
}
