using UnityEngine;
using Unity.Cinemachine;

//DEPRECATED
[RequireComponent(typeof(MeshRenderer))]
public class QuadParallaxTiling : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private bool debugScreenSize = false;
    private Vector2 ViewSize;
    private Vector2 startPos;
    [SerializeField] Vector2 parallaxEffect;

    [SerializeField] int sortingOrder = -10;
    [SerializeField] string sortingLayer = "Background2";

    Material mat;
    Vector2 tileSize, offset;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        var mr = GetComponent<MeshRenderer>();
        mat = mr.material;
        mr.sortingLayerName = sortingLayer;
        mr.sortingOrder = sortingOrder;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = cam.transform.position;
        resetViewSize();
        if(debugScreenSize)
        {
            Debug.Log(cam.orthographicSize * 2);
            Debug.Log($"Screen width: {ViewSize.x}, height: {ViewSize.y}"); 
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float newX = startPos.x + (cam.transform.position.x - startPos.x) * parallaxEffect.x;
        float newY = startPos.y + (cam.transform.position.y - startPos.y) * parallaxEffect.y;
        offset = new Vector2(newX, newY);
        setTextureProperties();
    }

    void setTextureProperties()
    {
        mat.mainTextureOffset = offset;
    }

    void resetViewSize()
    {
        Vector3 bl = cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 tr = cam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        ViewSize = tr - bl;
    }
}
