using System.IO;
using UnityEngine;

/* 
HOW TO USE THIS SCRIPT:
1. Create a new scene and make sure it is completely empty except for the camera. Make sure the camera is 
   positioned at (0, 0, <some negative number>).
2. Add the prefab you want to convert to this scene. Center the prefab at (0, 0, 0). If the pivot is misaligned
   (i.e. the arrows you use to move the prefab aren't at its center), you will have to manually move the prefab
   to (0, 0, 0) instead of setting the position in the inspector.
3. Add this script to the prefab that's in the scene. (Select the prefab and click on the "Add Component" button
   in the inspector. Type "Convert Prefab To PNG" in the search bar that pops up and click on the script.) 
4. Adjust the "Png Height" and "Png Width" values in the inspector. If your prefab is definitely less than 
   the default 512 x 512 pixels and you don't care about empty space on the edges you can skip this step.
5. Edit the "Png File Name" in the inspector if you want to name it something different than the default. 
6. Click the play button. You should see a debug statement confirming the prefab was converted successfully and 
   showing you where the resulting PNG is saved. 
7. The PNG will NOT be in the Unity project. It will be on your computer at the location specified in the debug
   statement.
8. Delete the new scene you just made.
*/

public class ConvertPrefabToPNG : MonoBehaviour
{
    public int pngWidth = 512;
    public int pngHeight = 512;
    public string pngFileName = "ConvertedPrefab.png";

    void Start()
    {
        ConvertPrefab();
    }

    void ConvertPrefab()
    {
        RenderTexture renderTexture = new RenderTexture(pngWidth, pngHeight, 24, RenderTextureFormat.ARGB32);
        Camera camera = Camera.main;

        camera.targetTexture = renderTexture;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;

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
}
