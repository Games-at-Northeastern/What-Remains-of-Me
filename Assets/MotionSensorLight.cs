using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Threading.Tasks;

[RequireComponent(typeof(Light2D))]
public class MotionSensorLight : MonoBehaviour
{
    [Header("This script only works with 2D spot lights!")]
    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private int minFlickers = 1;
    [SerializeField] private int maxFlickers = 3;
    [SerializeField] private int minFlickerLengthInMS = 10;
    [SerializeField] private int maxFlickerLengthInMS = 50;

    private float startingIntensity;
    private bool flickeringIn = false;
    private bool flickeringOut = false;
    private Light2D _light;
    private Transform _player;
    private Bounds bounds;
    private bool playerInBoundsLastFrame = false;
    private bool PlayerInBounds => bounds.Contains(_player.position);

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
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        var xDif = Mathf.Abs(Mathf.Cos(_light.pointLightOuterAngle / 2) * _light.pointLightOuterRadius);
        var topRight = new Vector2(gameObject.transform.position.x + xDif, gameObject.transform.position.y + _light.pointLightOuterRadius);
        var bottomLeft = new Vector2(gameObject.transform.position.x - xDif, gameObject.transform.position.y);

        var sinZRot = Mathf.Sin(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z);
        var cosZRot = Mathf.Cos(Mathf.Deg2Rad * gameObject.transform.localEulerAngles.z);
        var rot1 = RotatePointAroundPivot(topRight, gameObject.transform.position);
        var rot2 = RotatePointAroundPivot(bottomLeft, gameObject.transform.position);

        var maxX = Mathf.Max(rot1.x, rot2.x);
        var maxY = Mathf.Max(rot1.y, rot2.y);
        var minX = Mathf.Min(rot1.x, rot2.x);
        var minY = Mathf.Min(rot1.y, rot2.y);

        var boundsSize = new Vector3(
            Mathf.Abs(maxX - minX),
            Mathf.Abs(maxY - minY),
            0);
        var boundsCenter = new Vector3(
            minX + (boundsSize.x / 2),
            minY + (boundsSize.y / 2),
            0
        );
        bounds = new Bounds(boundsCenter, boundsSize);

        Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot)
        {
            Vector2 ret = new(
                (cosZRot * (point.x - pivot.x)) - (sinZRot * (point.y - pivot.y)) + pivot.x,
                (sinZRot * (point.x - pivot.x)) + (cosZRot * (point.y - pivot.y)) + pivot.y
                );
            Debug.Log($"MDB -- {gameObject.name}: {point} rotated {gameObject.transform.eulerAngles.z} degrees around {pivot} = {ret}");
            return ret;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (bounds == null)
        {
            return;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    private void Update()
    {
        var inBounds = PlayerInBounds;
        if (inBounds == playerInBoundsLastFrame)
        {
            return;
        }
        playerInBoundsLastFrame = inBounds;
        if (inBounds)
        {
            FlickerIn();
        }
        else
        {
            FlickerOut();
        }
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
