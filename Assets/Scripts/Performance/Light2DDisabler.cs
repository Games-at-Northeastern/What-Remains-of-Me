using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class Light2DDisabler : PlayerDistanceComponentDisabler<Light2D>
{
    protected override Bounds GetComponentBounds(Light2D light)
    {
        switch (light.lightType)
        {
            case Light2D.LightType.Global:
            case Light2D.LightType.Parametric:
            {
                break;
            }
            case Light2D.LightType.Sprite:
            {
                return light.lightCookieSprite.bounds;
            }
            case Light2D.LightType.Freeform:
            {
                return GetFreeformLightBounds(light);
            }
            case Light2D.LightType.Point:
            {
                return GetPointLightBounds(light);
            }
        }
        Debug.LogError($"Unable to get light bounds for {light.gameObject.name}");
        return new Bounds();
    }

    private Bounds GetFreeformLightBounds(Light2D light)
    {
        Vector3[] points = light.shapePath;
        Vector3 offset = light.transform.position - light.transform.localPosition;
        Bounds bounds = new Bounds(new Vector3(points[0].x + offset.x, points[0].y + offset.y), Vector3.zero);

        for (int i = 1; i < points.Length; i++)
        {
            bounds.Encapsulate(new Vector3(points[i].x + offset.x, points[i].y + offset.y));
        }

        return bounds;
    }

    private Bounds GetPointLightBounds(Light2D light)
    {
        var angleOver180 = Mathf.Abs(light.pointLightOuterAngle) >= 180f;
        var maxXFromCenter = angleOver180 ? light.pointLightOuterRadius : Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * light.pointLightOuterAngle / 2f) * light.pointLightOuterRadius);
        var extendBelowGameObjY = angleOver180 ? light.pointLightOuterRadius * Mathf.Sin((Mathf.Deg2Rad * light.pointLightOuterAngle / 2f) - (Mathf.Deg2Rad * 90f)) : 0f;

        Vector3 bottomLeft = new(
            light.transform.position.x - maxXFromCenter,
            light.transform.position.y - extendBelowGameObjY,
            0
        );
        Vector3 topRight = new(
            light.transform.position.x + maxXFromCenter,
            light.transform.position.y + light.pointLightOuterRadius,
            0
        );

        var sinZRot = Mathf.Sin(Mathf.Deg2Rad * light.transform.eulerAngles.z);
        var cosZRot = Mathf.Cos(Mathf.Deg2Rad * light.transform.eulerAngles.z);
        Vector3 bottomLeftRot = RotatePointAroundPivot(bottomLeft, light.transform.position);
        Vector3 topRightRot = RotatePointAroundPivot(topRight, light.transform.position);

        Bounds bounds = new(bottomLeftRot, Vector3.zero);
        bounds.Encapsulate(topRightRot);
        return bounds;

        Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot) => new(
            (cosZRot * (point.x - pivot.x)) - (sinZRot * (point.y - pivot.y)) + pivot.x,
            (sinZRot * (point.x - pivot.x)) + (cosZRot * (point.y - pivot.y)) + pivot.y
            );
    }

    public void AddAllSceneComponents()
    {
        components.Clear();
        Light2D[] lights = FindObjectsOfType<Light2D>(true);
        foreach (var l in lights)
        {
            if (l.lightType == Light2D.LightType.Global)
                continue;
            components.Add(l);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Light2DDisabler))]
public class Light2DDisablerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Add all 2D lights in scene"))
        {
            var disabler = (Light2DDisabler)target;
            disabler.AddAllSceneComponents();
        }
    }
}
#endif