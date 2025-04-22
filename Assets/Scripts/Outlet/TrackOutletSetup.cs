using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A class that set ups a full track of moving outlets acros a set of points
/// </summary>
public class TrackOutletSetup : MonoBehaviour
{
    //Transform parent of all the points of the track(assumed that all points are the children of the transform)
    [SerializeField] private Transform pointParent;

    //Transform parent of all the moving outlets(assumed that all outlets are the children of the transform)
    [SerializeField] private Transform outletParent;
    [SerializeField] private bool isMovingRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //For every outlet, make them secondaries of each other
        int elementChildren = 0;
        foreach (Transform child in outletParent)
        {
            if (child.TryGetComponent(out Outlet outlet))
            {
                elementChildren++;
                foreach (Transform child2 in outletParent)
                {

                    if (child2.TryGetComponent(out Outlet outlet2) && outlet2 != outlet)
                    {
                        outlet.AddSecondary(outlet2.GetComponent<AControllable>());
                    }
                }

            }
        }
        List<Transform> points = new List<Transform>();



        //Add all the points in order, and then find the totoal distance of the track
        float distance = 0;
        points.Add(pointParent.GetChild(0));
        for (int transformIndex = 1; transformIndex < pointParent.childCount; transformIndex++)
        {
            distance += Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
            points.Add(pointParent.GetChild(transformIndex));
        }
        distance += Vector2.Distance(pointParent.GetChild(0).position, pointParent.GetChild(pointParent.childCount - 1).position);



        //For each of the moving outlets, find which one they are across the distance, then set their position
        int movingElementNumber = 0;
        for (int transformIndex = 0; transformIndex < outletParent.childCount; transformIndex++)
        {
            if (outletParent.GetChild(transformIndex).TryGetComponent(out MovingElement element) && element.gameObject.activeInHierarchy)
            {
                movingElementNumber++;
                SetTrackPosition(element, movingElementNumber, elementChildren, distance, points);
                element.Init();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element">The movign element to be set up.</param>
    /// <param name="index">Index of the element out of the element amount(used to calculate what position it'll end up</param>
    /// <param name="elementAmount">The amount of total elements to get a percetange out of</param>
    /// <param name="trackLength">The length of the total track</param>
    /// <param name="newPoints">The actual set of points passed to the outlet</param>
    private void SetTrackPosition(MovingElement element, int index, int elementAmount, float trackLength, List<Transform> newPoints)
    {
        //Find how far across far the element should be (still in regular unity units)
        float distance = trackLength * (index / (float)elementAmount);
        for (int transformIndex = 1; transformIndex < pointParent.childCount; transformIndex++)
        {
            //Keep taking off from the total distance across each point till we find the two points the player is inbetween
            if (distance > Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position))
            {
                distance -= Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
            }
            else
            {

                int adjustedIndex = transformIndex - 1;
                //Find how far the point should be between the two points, while also randomizing it a bit(+- 1.5 unity units), then turn it into a percentage
                float adjustedNormal = (distance + Random.Range(-1.5f, 1.5f)) / Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
                adjustedNormal %= 1;
                if (!isMovingRight)
                {
                    adjustedIndex = adjustedIndex + 1;
                    adjustedNormal = 1 - adjustedNormal;
                }

                //Reset the position of the element with this new normalized distance.
                element.ResetPositioning(adjustedNormal, adjustedIndex, isMovingRight, newPoints);
                return;
            }
        }


        //Do the same operation for the last element(kept outside since it used the first and last point, so it'd break the loop
        int newIndex = pointParent.childCount - 1;
        float newNormal = distance / Vector2.Distance(pointParent.GetChild(0).position, pointParent.GetChild(pointParent.childCount - 1).position);
        if (!isMovingRight)
        {
            newIndex = 0;
            newNormal = 1 - newNormal;
        }
        element.ResetPositioning(newNormal, newIndex, isMovingRight, newPoints);
        return;
    }
}
