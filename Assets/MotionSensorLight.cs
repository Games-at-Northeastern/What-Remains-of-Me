using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;

[RequireComponent(typeof(Light2D))]
public class MotionSensorLight : MonoBehaviour
{
    [Header("This script only works with point lights that are facing down!")]
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private int minFlickers = 2;
    [SerializeField] private int maxFlickers = 5;
    [SerializeField] private int minFlickerLengthInMS = 10;
    [SerializeField] private int maxFlickerLengthInMS = 50;

    private Vector2 minCoords;
    private Vector2 maxCoords;
    private float startingIntensity;
    private bool flickeringIn = false;
    private bool flickeringOut = false;
    private Light2D _light;
    private Transform _player;
    private bool playerInBoundsLastFrame = false;
    private bool PlayerInBounds => _player != null
            && _player.position.x > minCoords.x
            && _player.position.x < maxCoords.x
            && _player.position.y > minCoords.y
            && _player.position.y < maxCoords.y;

    private void Awake()
    {
        if (TryGetComponent(out _light))
        {
            _light.enabled = false;
            startingIntensity = _light.intensity;
        }
        else
        {
            Debug.LogError("Could not locate light");
            return;
        }
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        var xDif = Mathf.Abs(Mathf.Cos(_light.pointLightOuterAngle / 2) * _light.pointLightOuterRadius);
        maxCoords = new Vector2(gameObject.transform.position.x + xDif, gameObject.transform.position.y);
        minCoords = new Vector2(gameObject.transform.position.x - xDif, maxCoords.y - _light.pointLightOuterRadius);
    }

    private void Update()
    {
        var inBounds = PlayerInBounds;
        if (inBounds == playerInBoundsLastFrame)
        {
            return;
        }

        if (inBounds)
        {
            FlickerIn();
        }
        else
        {
            FlickerOut();
        }
        playerInBoundsLastFrame = inBounds;
    }

    private async void FlickerOut()
    {
        if (flickeringIn)
        {
            while (flickeringIn)
            {
                await Task.Yield();
            }
            await Task.Delay(minFlickerLengthInMS);
            _light.enabled = false;
            return;
        }

        flickeringOut = true;
        await Flicker(false);
        flickeringOut = false;
    }

    private async void FlickerIn()
    {
        if (flickeringOut)
        {
            while (flickeringOut)
            {
                await Task.Yield();
            }
            await Task.Delay(minFlickerLengthInMS);
            _light.intensity = startingIntensity;
            _light.enabled = true;
            return;
        }

        flickeringIn = true;
        await Flicker(true);
        flickeringIn = false;
    }

    private async Task Flicker(bool endOn)
    {
        var flickerCount = Random.Range(minFlickers, maxFlickers + 1);
        if (_light.enabled)
        {
            await SingleFlicker(false);
        }

        for (int i = 0; i < flickerCount * 2; i++)
        {
            await SingleFlicker(i % 2 == 0);
        }

        _light.enabled = endOn;
        _light.intensity = startingIntensity;
    }

    private async Task SingleFlicker(bool on)
    {
        var duration = Random.Range(minFlickerLengthInMS, maxFlickerLengthInMS + 1);
        if (!on)
        {
            _light.enabled = false;
            await Task.Delay(duration);
        }
        else
        {
            _light.intensity = Random.Range(minIntensity, maxIntensity);
            _light.enabled = true;
            await Task.Delay(duration);
        }
    }
}
