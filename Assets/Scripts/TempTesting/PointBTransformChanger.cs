using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBTransformChanger : MonoBehaviour
{
    [SerializeField] private Transform t;
    private bool changed = false;
    [SerializeField] private Vector3 change;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTransform(MovingElementController mec)
    {
        if (mec.GetVirusPercent() > .7)
        {
            if (!changed)
            {
                changed = true;
                t.position += change;
            }
        }
    }
}
