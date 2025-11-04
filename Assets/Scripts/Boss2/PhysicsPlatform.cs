using Pathfinding;
using UnityEngine;

public class PhysicsPlatform : MonoBehaviour
{
    private MovingElement thisElement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       thisElement = GetComponent<MovingElement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisElement._destinationIndex == 0)
        {
            thisElement.Deactivate();
        }

    }
}
