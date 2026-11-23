using System.Collections.Generic;
using UnityEngine;

// One depth layer of background fish. Spawns swarms that swim across the camera view
public class FishWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Species
    {
        public Sprite sprite;
        public bool facesRight = true;
        [Tooltip("Higher value gets picked more often.")]
        [Min(0f)] public float weight = 1f;
        public Vector2Int swarmSize = new Vector2Int(3, 8);
        public float speedMultiplier = 1f;
    }

    private class Fish
    {
        public SpriteRenderer sprite;
        public float speed;
        public float baseY;
        public float bobOffset;
        public float despawnDistance;
    }

    [SerializeField] private Species[] species;

    [Header("Depth")]
    [SerializeField] private float fishScale = 1f;
    [SerializeField] private Color tint = Color.white;
    [SerializeField] private string sortingLayerName = "Background 3";
    [SerializeField] private int sortingOrder = 99;
    [SerializeField] private Material fishMaterial;
    [Tooltip("0 = stays in the world, 1 = moves with the camera. Farther layers should be higher.")]
    [SerializeField, Range(0f, 1f)] private float parallaxX = 0.05f;
    [SerializeField, Range(0f, 1f)] private float parallaxY = 0.1f;

    [Header("Swimming")]
    [SerializeField] private Vector2 swimSpeed = new Vector2(1.5f, 2.5f);
    [SerializeField] private float bobAmplitude = 0.08f;
    [SerializeField] private float bobFrequency = 1.5f;
    [Tooltip("Delay between fish in a swarm, so the bob ripples through it.")]
    [SerializeField] private float bobRipple = 0.6f;
    [SerializeField, Range(0f, 1f)] private float leftToRightChance = 0.5f;

    [Header("Waves")]
    [SerializeField] private Vector2 timeBetweenWaves = new Vector2(2f, 5f);
    [Tooltip("Gap between fish in a swarm in fish widths.")]
    [SerializeField] private float swarmSpacing = 1.3f;
    [Tooltip("Where swarms spawn vertically: -0.5 = bottom, 0.5 = top of screen.")]
    [SerializeField] private Vector2 verticalBand = new Vector2(-0.45f, 0.45f);
    [Tooltip("Fill a view to this size so zooming out never shows empty space.")]
    [SerializeField] private float coverCameraSize = 0f;
    [SerializeField] private int maxFish = 60;
    [SerializeField] private float prewarmSeconds = 60f;

    private readonly List<Fish> swimming = new List<Fish>();
    private readonly Stack<Fish> unused = new Stack<Fish>();
    private Camera cam;
    private Vector2 startPosition;
    private float nextWaveTime;

    void Start()
    {
        cam = Camera.main;
        startPosition = transform.position;

        FollowCamera();
        Prewarm();
        nextWaveTime = Time.time + RandomBetween(timeBetweenWaves);
    }

    //prespawns fishes
    private void Prewarm()
    {
        const float step = 0.1f;
        float timeUntilNextSwarm = 0f;

        for (float t = 0f; t < prewarmSeconds; t += step)
        {
            timeUntilNextSwarm -= step;
            if (timeUntilNextSwarm <= 0f)
            {
                SpawnSwarm();
                timeUntilNextSwarm = RandomBetween(timeBetweenWaves);
            }
            MoveFish(step);
        }
    }

    // LateUpdate since camera moves in LateUpdate
    void LateUpdate()
    {
        if (cam == null) return;

        FollowCamera();

        if (Time.time >= nextWaveTime)
        {
            SpawnSwarm();
            nextWaveTime = Time.time + RandomBetween(timeBetweenWaves);
        }

        MoveFish(Time.deltaTime);
    }

    private void FollowCamera()
    {
        Vector2 camPos = cam.transform.position;
        float x = startPosition.x + (camPos.x - startPosition.x) * parallaxX;
        float y = startPosition.y + (camPos.y - startPosition.y) * parallaxY;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void MoveFish(float deltaTime)
    {
        Vector2 camPos = CameraPositionInLayer();
        float screenHeight = CoveredHalfHeight() * 2f;

        // Backwards so fish can be removed while looping.
        for (int i = swimming.Count - 1; i >= 0; i--)
        {
            Fish fish = swimming[i];
            Vector3 pos = fish.sprite.transform.localPosition;
            pos.x += fish.speed * deltaTime;
            pos.y = fish.baseY + Mathf.Sin(Time.time * bobFrequency + fish.bobOffset) * bobAmplitude;
            fish.sprite.transform.localPosition = pos;

            //check y since camera moves up and down
            bool offScreen = Mathf.Abs(pos.x - camPos.x) > fish.despawnDistance
                          || Mathf.Abs(pos.y - camPos.y) > screenHeight * 1.5f;
            if (offScreen)
            {
                fish.sprite.gameObject.SetActive(false);
                swimming.RemoveAt(i);
                unused.Push(fish);
            }
        }
    }

    private void SpawnSwarm()
    {
        Species type = PickRandomSpecies();
        if (type == null) return;

        int count = Mathf.Min(Random.Range(type.swarmSize.x, type.swarmSize.y + 1), maxFish - swimming.Count);
        if (count <= 0) return;

        Vector2 camPos = CameraPositionInLayer();
        float halfScreenHeight = CoveredHalfHeight();
        float halfScreenWidth = halfScreenHeight * cam.aspect;

        bool goingRight = Random.value < leftToRightChance;
        float direction = goingRight ? 1f : -1f;
        float speed = RandomBetween(swimSpeed) * type.speedMultiplier;

        float fishWidth = type.sprite.bounds.size.x * fishScale;
        float fishHeight = type.sprite.bounds.size.y * fishScale;
        float swarmLength = fishWidth * swarmSpacing * Mathf.Max(1f, count * 0.6f);
        float swarmHeight = fishHeight * 1.2f * Mathf.Sqrt(count);

        float leaderX = camPos.x - direction * (halfScreenWidth + fishWidth);
        float leaderY = camPos.y + RandomBetween(verticalBand) * halfScreenHeight * 2f;
        float leaderBob = Random.Range(0f, Mathf.PI * 2f);

        for (int i = 0; i < count; i++)
        {
            bool isLeader = i == 0;
            float behindLeader = isLeader ? 0f : Random.Range(0.3f, 1f) * swarmLength;
            float aboveLeader = isLeader ? 0f : Random.Range(-0.5f, 0.5f) * swarmHeight;

            Fish fish = GetFish();
            fish.sprite.sprite = type.sprite;
            fish.sprite.flipX = goingRight != type.facesRight;
            fish.speed = direction * speed * Random.Range(0.95f, 1.05f);
            fish.baseY = leaderY + aboveLeader;
            fish.bobOffset = leaderBob + i * bobRipple;
            fish.despawnDistance = halfScreenWidth + swarmLength + fishWidth * 2f;
            fish.sprite.transform.localPosition = new Vector3(leaderX - direction * behindLeader, fish.baseY, 0f);
            fish.sprite.gameObject.SetActive(true);
            swimming.Add(fish);
        }
    }

    private Species PickRandomSpecies()
    {
        float totalWeight = 0f;
        foreach (Species s in species)
        {
            if (s.sprite != null) totalWeight += s.weight;
        }

        float roll = Random.value * totalWeight;
        foreach (Species s in species)
        {
            if (s.sprite == null) continue;
            roll -= s.weight;
            if (roll <= 0f) return s;
        }
        return null;
    }

    // Reuses a fish that swam off screen, or makes a new one.
    private Fish GetFish()
    {
        Fish fish;
        if (unused.Count > 0)
        {
            fish = unused.Pop();
        }
        else
        {
            GameObject go = new GameObject("Fish");
            go.transform.SetParent(transform, false);
            fish = new Fish { sprite = go.AddComponent<SpriteRenderer>() };
            if (fishMaterial != null) fish.sprite.sharedMaterial = fishMaterial;
        }

        fish.sprite.color = tint;
        fish.sprite.sortingLayerName = sortingLayerName;
        fish.sprite.sortingOrder = sortingOrder;
        fish.sprite.transform.localScale = new Vector3(fishScale, fishScale, 1f);
        return fish;
    }

    private float CoveredHalfHeight()
    {
        return Mathf.Max(cam.orthographicSize, coverCameraSize);
    }

    private Vector2 CameraPositionInLayer()
    {
        return transform.InverseTransformPoint(cam.transform.position);
    }

    private static float RandomBetween(Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
}
