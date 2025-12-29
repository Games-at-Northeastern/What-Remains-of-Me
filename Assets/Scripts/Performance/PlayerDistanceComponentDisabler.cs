using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Disables components of type T if the player game object is far enough away. Checks distance between
/// player and components every 'X' frames. Distance is calculated using Bounds, which are computed and 
/// cached OnAwake. Extending classes define logic for computing bounds by overriding 'GetComponentBounds'. 
/// </summary>
/// <typeparam name="T"> the component type </typeparam>
public class PlayerDistanceComponentDisabler<T> : PlayerDistanceComponentDisabler where T : MonoBehaviour
{
    [SerializeField] protected uint checkEveryXFrames = 8;
    [SerializeField] protected float disableDistance = 20f;
    [SerializeField] protected List<T> components = new();
    [Space]
    [SerializeField] private bool showDisabledBounds = false;

    private List<(Bounds, T)> componentBounds = new();
    private int frame = 0;
    private Transform player;


    private void Awake()
    {
        foreach (var comp in components)
        {
            if (!comp)
                continue;

            Bounds bounds = GetComponentBounds(comp);
            componentBounds.Add(new(bounds, comp));
        }
    }

    private void Update()
    {
        if (frame == 0)
        {
            CheckComponents();
        }
        frame = (int)((frame + 1) % checkEveryXFrames);
    }

    private void CheckComponents()
    {
        player ??= GameObject.FindGameObjectWithTag("Player").transform;
        if (!player)
            return;

        foreach (var pair in componentBounds)
        {
            Vector3 closestPointOnBounds = pair.Item1.ClosestPoint(player.position);
            float boundsDistance = Mathf.Abs(Vector3.Distance(closestPointOnBounds, player.position));

            if (pair.Item2.enabled && boundsDistance >= disableDistance)
            {
                pair.Item2.enabled = false;
            }
            else if (!pair.Item2.enabled && boundsDistance < disableDistance)
            {
                pair.Item2.enabled = true;
            }
        }
    }

    protected virtual Bounds GetComponentBounds(T comp)
    {
        if (comp.TryGetComponent<Collider2D>(out var collider))
        {
            return collider.bounds;
        }
        Debug.LogError($"Could not get bounds for {comp.gameObject.name}");
        return new Bounds();
    }

    private void OnDrawGizmosSelected()
    {
        if (componentBounds == null || componentBounds.Count == 0)
        {
            return;
        }

        foreach (var pair in componentBounds)
        {
            if (pair.Item2.enabled)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pair.Item1.center, pair.Item1.size);
            }
            else if (showDisabledBounds)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(pair.Item1.center, pair.Item1.size);
            }
        }
    }

    public override void AddAllSceneComponents()
    {
        components.AddRange(FindObjectsOfType<T>(true));
    }

    internal void AddDisableableComponent(T comp)
    {
        components.Add(comp);
    }
}

public abstract class PlayerDistanceComponentDisabler : MonoBehaviour
{
    public abstract void AddAllSceneComponents();
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerDistanceComponentDisabler))]
public class PlayerDistanceComponentDisablerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add all scene components"))
        {
            var disabler = (PlayerDistanceComponentDisabler)target;
            disabler.AddAllSceneComponents();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif