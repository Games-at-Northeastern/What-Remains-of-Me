using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    private List<GameObject> backgroundElements;

    public float elementSize;
    public float elementOffset;
    public GameObject referenceObject;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        backgroundElements = new List<GameObject>();

        // getting number of children objects under the backgroundElements parent object
        for (int i = 0; i < transform.childCount; i++)
        {
            backgroundElements.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // applying the parallax effect
        foreach (GameObject backgroundElement in backgroundElements)
        {
            if (referenceObject.transform.position.x - backgroundElement.transform.position.x > elementOffset)
            {
                backgroundElement.transform.position = new Vector2 (
                    backgroundElement.transform.position.x + backgroundElements.Count * elementSize,
                    backgroundElement.transform.position.y
                    );
            }
        }

        // making BG move
        foreach (GameObject backgroundElement in backgroundElements)
        {
            backgroundElement.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }
}
