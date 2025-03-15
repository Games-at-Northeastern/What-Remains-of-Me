using System.IO;
using UnityEngine;

/* 
HOW TO USE THIS SCRIPT:
1.  Create a new scene and make sure it is completely empty except for the camera. Make sure the camera is 
    positioned at (0, 0, <some negative number>).
2.  Add the prefab you want to convert to the scene. 
3.  Create a new empty game object and add this script to it. (Select the object and click on the "Add Component" 
    button in the inspector. Type "Convert Prefab To PNG" in the search bar that pops up and click on the script.)
4.  The empty game object (with this script) will be the new pivot, so center it over the prefab. Make all the 
    contents of the prefab children of the empty game object. 
5.  Disable the original prefab. 
6.  Position the empty game object (which is now the parent of all the prefab's contents) at (0, 0, 0).
7.  In the "Convert Prefab to PNG" component settings in the inspector, change "Png File Name" if you want to name
    the result PNG something other than the default. 
8.  If "Snap Rotations to Right Angles" is not enabled, the resulting PNG might look slightly different from the 
    prefab on the screen because this script essentially re-pixelizes the prefab. If a pixel art sprite's Z-
    rotation isn't a multiple of 90, its pixels don't line up with the grid because its edges are no longer 
    perfectly horizontal/vertical. If enabled, this script changes the Z-rotation of all sprites in the prefab to 
    the nearest right angle (+/- 0, 90, 180, 270, 360).
        * Enabling "Snap Rotations to Right Angles" preserves the detail of the original pixel art but changes
          the position of some sprites 
          Disabling "Snap Rotations to Right Angles" preserves the position of all sprites but some detail of the
          original pixel art might be lost *
    If you enable "Snap Rotations to Right Angles", you have to restructure the sprites so that every single sprite 
    is an immediate child of the empty game object. This is to ensure that each sprite is rotated once to the closest 
    right angle instead of being rotated multiple times when its parent(s) are rotated.
    The prefab SHOULD look like this after restructuring:
        ˇ Empty game object
            Sprite 1
            Sprite 2
            Sprite 3
            etc. 
    The prefab should NOT look like this: 
        ˇ Non-empty game object
            ˇ Sprite 1
                Sprite 2
            ˇ Empty game object 
                Sprite 3
            etc. 
9.  Click the play button. You should see a debug statement confirming the prefab was converted successfully and 
    showing you where the resulting PNG is saved. 
10. The PNG will NOT be in the Unity project. It will be on your computer at the location specified in the debug
    statement.
11. Delete the new scene you just made.
*/

public class ConvertPrefabToPNG : MonoBehaviour
{
    public bool snapRotationsToRightAngles = true;
    public bool convertPrefab = true;
    public string pngFileName = "ConvertedPrefab.png";
    int pngWidth;
    int pngHeight;
    Bounds bounds;
    Camera camera;

    void Start()
    {
        camera = Camera.main;

        if (snapRotationsToRightAngles)
            SnapRotations();

        if (convertPrefab)
            ConvertPrefab();
    }

    void ConvertPrefab()
    {
        CalculateBounds();
        SetPNGSize();

        RenderTexture renderTexture = new RenderTexture(pngWidth, pngHeight, 24, RenderTextureFormat.ARGB32);

        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;
        camera.aspect = (float)pngWidth / pngHeight;
        
        SetCameraSize();
        
        camera.Render();

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        camera.targetTexture = null;

        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, pngFileName);
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Prefab converted successfully, png saved to: " + filePath);
    }

    void CalculateBounds()
    {
        bounds = new Bounds(transform.position, Vector3.zero);
        if (gameObject.GetComponent<Renderer>())
            bounds = GetComponent<Renderer>().bounds;

        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
        {
            bounds.Encapsulate(spriteRenderer.bounds);
        }
    }

    void SetCameraSize()
    {
        float aspectRatio = (float)pngWidth / pngHeight;
        float prefabWidth = bounds.size.x;
        float prefabHeight = bounds.size.y;

        if (prefabWidth > prefabHeight * aspectRatio)
        {
            camera.orthographicSize = prefabWidth / aspectRatio / 2f;
        }
        else
        {
            camera.orthographicSize = prefabHeight / 2f;
        }

        camera.transform.position = new Vector3(bounds.center.x, bounds.center.y, -10);
    }

    void SetPNGSize()
    {
        float pixelsPerUnit = 32f;

        pngWidth = Mathf.CeilToInt(bounds.size.x * pixelsPerUnit);
        pngHeight = Mathf.CeilToInt(bounds.size.y * pixelsPerUnit);
    }

    void SnapRotations()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>())
            {
                float currentZRotation = child.rotation.eulerAngles.z;
                float closestAngle = ClosestRightAngle(currentZRotation);
                child.rotation = Quaternion.Euler(child.rotation.eulerAngles.x, child.rotation.eulerAngles.y, closestAngle);
            }
        }
    }

    float ClosestRightAngle(float currentRotation)
    {
        float[] rightAngles = { -360, -270, -180, -90, 0, 90, 180, 270, 360 };
        currentRotation = (currentRotation + 360) % 360;

        float closestRightAngle = rightAngles[0];
        float smallestDifference = Mathf.Abs(currentRotation - closestRightAngle);

        foreach (float rightAngle in rightAngles)
        {
            float difference = Mathf.Abs(currentRotation - rightAngle);
            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closestRightAngle = rightAngle;
            }
        }

        return closestRightAngle;
    }
}
