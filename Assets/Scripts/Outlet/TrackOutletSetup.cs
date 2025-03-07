using UnityEngine;
using System.Collections.Generic;
public class TrackOutletSetup : MonoBehaviour
{
    [SerializeField] private Transform pointParent;
    [SerializeField] private Transform outletParent;
    [SerializeField] private bool isMovingRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                        Debug.Log("here");
                        outlet.AddSecondary(outlet2.GetComponent<AControllable>());
                    }
                }

            }
        }
        List<Transform> points = new List<Transform>();


        float distance = 0;
        points.Add(pointParent.GetChild(0));
        for (int transformIndex = 1; transformIndex < pointParent.childCount; transformIndex++)
        {
            distance += Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
            points.Add(pointParent.GetChild(transformIndex));
        }
        distance += Vector2.Distance(pointParent.GetChild(0).position, pointParent.GetChild(pointParent.childCount - 1).position);

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


    private void SetTrackPosition(MovingElement element, int index, int elementAmount, float trackLength, List<Transform> newPoints)
    {
        float distance = trackLength * (index / (float)elementAmount);
        for (int transformIndex = 1; transformIndex < pointParent.childCount; transformIndex++)
        {
            if (distance > Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position))
            {
                distance -= Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
            }
            else
            {
                int adjustedIndex = transformIndex - 1;
                
                float adjustedNormal = distance / Vector2.Distance(pointParent.GetChild(transformIndex).position, pointParent.GetChild(transformIndex - 1).position);
                if (!isMovingRight)
                {
                    adjustedIndex = adjustedIndex + 1;
                    adjustedNormal = 1 - adjustedNormal;
                }
                element.ResetPositioning(adjustedNormal, adjustedIndex, isMovingRight, newPoints);
                return;
            }
        }

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
