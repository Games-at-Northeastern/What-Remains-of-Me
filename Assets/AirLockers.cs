using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Transform))]
public class AirLockers : MonoBehaviour
{

    // This script is currently for airlock doors that close vertically, not horizontally.


    Vector2 startingPos;


    // Travel distance of the door.
    [SerializeField]
    private float moveDistance = 1f;

    // reverses the direction the door moves depending on if it is on the ceiling or floor.
    [SerializeField]
    private bool isTopLocker;

    // How fast the doors are moving.
    [Header("Moving Speeds")]
    [SerializeField]
    private float closeSpeed = 5f;
    [SerializeField]
    private float openSpeed = 0.5f;
    private float closeTime = 0;
    private float openTime = 0;


    private bool moveBack = false;
    private bool waiting = false;

    // Time to wait after movement.
    [Header("Wait times")]
    [SerializeField]
    private float stationatyWait = 3f;
    [SerializeField]
    private float fallenWait = 1.5f;

    // Delay before starting movement(offset).
    [SerializeField]
    private float startDelay = 0.5f;

    // One plays when slamming down, the other when moving back up.
    [Header("Audio")]
    [SerializeField]
    private AudioSource sfxSlam;
    [SerializeField]
    private AudioSource sfxReturn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
        waiting = true;
        StartCoroutine(StartingWait());
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting)
        {
            return;
        }

        Vector2 moveTo;

        if (isTopLocker)
        {
            moveTo = new Vector2(startingPos.x, startingPos.y - moveDistance);
        }
        else
        {
            moveTo = new Vector2(startingPos.x, startingPos.y + moveDistance);
        }


        closeTime += closeSpeed * Time.deltaTime;
        openTime += openSpeed * Time.deltaTime;

        Vector2 newPos;
        if (!moveBack)
        {
            newPos = Vector2.Lerp(startingPos, moveTo, closeTime);
        }
        else
        {
            newPos = Vector2.Lerp(moveTo, startingPos, openTime);
        }

        transform.position = newPos;

        Vector2 target;

        if (moveBack)
        {
            target = startingPos;
        }
        else
        {
            target = moveTo;
        }


        if (Vector2.Distance(newPos, target) < 0.01f)
        {
            StartCoroutine(WaitAtTarget());
        }
       
    }


    IEnumerator WaitAtTarget()
    {
        waiting = true;
        if (moveBack)
        {
            yield return new WaitForSeconds(stationatyWait);
            sfxSlam.Play();
        }
        else
        {
            yield return new WaitForSeconds(fallenWait);
            sfxReturn.Play();
        }
        moveBack = !moveBack;
        closeTime = 0f;
        openTime = 0f;
        waiting = false;
    }


    IEnumerator StartingWait()
    {
        yield return new WaitForSeconds(startDelay);
        waiting = false;
        sfxSlam.Play();
    }

}
