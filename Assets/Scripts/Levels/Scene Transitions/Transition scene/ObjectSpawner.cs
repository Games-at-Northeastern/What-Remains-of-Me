using System.Collections;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // Assign the object prefab in the inspector
    public float spawnInterval = 2f; // Time interval for spawning
    public float objectSpeed = 2f; // Speed at which objects move down
    public float objectLifetime = 5f; // Time after which object gets destroyed
    public string sortingLayerName = "Default"; // Sorting layer set in Unity Inspector
    public int baseSortingOrder = 0; // Base sorting order for objects

    private int currentSortingOrder; // Variable to track sorting order

    private void Start()
    {
        currentSortingOrder = baseSortingOrder; // Initialize sorting order
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Instantiate the new object
            GameObject newObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
            
            // Set the sorting layer and sorting order for the sprite renderer
            SpriteRenderer spriteRenderer = newObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = sortingLayerName; // Set to the specified layer
                spriteRenderer.sortingOrder = currentSortingOrder; // Set order in layer
            }


            // Add movement and lifetime functionality
            newObject.AddComponent<MovingObject>().SetSpeed(objectSpeed, objectLifetime);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

public class MovingObject : MonoBehaviour
{
    private float speed;
    private float lifetime;

    public void SetSpeed(float moveSpeed, float lifeTime)
    {
        speed = moveSpeed;
        lifetime = lifeTime;
        Destroy(gameObject, lifetime); // Destroy object after a set lifetime
    }

    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}
