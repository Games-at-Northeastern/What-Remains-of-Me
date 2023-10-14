using UnityEngine;

public class PowerTrail : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    public static GameObject Setup(string name, LineRenderer renderer)
    {
        var g = new GameObject
        {
            name = name
        };

        var mf = g.AddComponent<MeshFilter>();
        var mr = g.AddComponent<MeshRenderer>();

        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        // TODO: make the materials automated as well? Meh.

        g.AddComponent<PowerTrail>();

        Mesh m = new Mesh(); // does this cause a memory leak? Hm.
        m.name = $"Custom LineRender #{m.GetHashCode()}";

        renderer.BakeMesh(m);

        mf.sharedMesh = m;

        return g;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetActiveStatus(bool enabled)
    {

    }

    public void SetVirus()
    {

    }
}
