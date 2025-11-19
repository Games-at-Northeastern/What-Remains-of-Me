using UnityEngine;

public class WallToPlayer : MonoBehaviour
{
    // assume right-facing alignment
    [SerializeField] private Vector3 alignedIfPlayerAtPos_1;
    [SerializeField] private Vector3 alignedIfWallAtPos_1;
    [SerializeField] private Vector3 alignedIfPlayerAtPos_2;
    [SerializeField] private Vector3 alignedIfWallAtPos_2;
    private Transform player;
    private Vector3 wallRate;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        CalculateWallRate();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }
        var playerDelta = player.position - alignedIfPlayerAtPos_1;
        gameObject.transform.position = new Vector3(
            alignedIfWallAtPos_1.x + (wallRate.x * playerDelta.x),
            alignedIfWallAtPos_1.y + (wallRate.y * playerDelta.y),
            1);
    }

    private void CalculateWallRate()
    {
        var playerDistance = alignedIfPlayerAtPos_2 - alignedIfPlayerAtPos_1;
        var wallDistance = alignedIfWallAtPos_2 - alignedIfWallAtPos_1;
        wallRate = new Vector3(wallDistance.x / playerDistance.x, wallDistance.y / playerDistance.y, 1);
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
