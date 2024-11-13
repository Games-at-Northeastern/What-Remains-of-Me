using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectParallax : MonoBehaviour
{
    [SerializeField] private float xFactor;
    [SerializeField] private float yFactor;
    [SerializeField] private bool parallaxX;
    [SerializeField] private bool parallaxY;

    private GameObject cam;
    private Vector3 initialPos;

    void Start()
    {
        cam = Camera.main.gameObject;
        initialPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    private void LateUpdate()
    {
        Vector3 diff = cam.transform.position - initialPos;

        Debug.Log(cam.transform.position);

        float newX = parallaxX ? initialPos.x + (diff.x * xFactor) : initialPos.x;
        float newY = parallaxY ? initialPos.y + (diff.y * yFactor) : initialPos.y;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
