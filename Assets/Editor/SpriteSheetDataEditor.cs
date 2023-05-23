using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteSheetData))]
public class SpriteSheetDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Convert Sprite Sheet"))
        {
            ConvertSpriteSheet();
        }
    }

    private void ConvertSpriteSheet()
    {
        SpriteSheetData spriteSheetData = (SpriteSheetData)target;

        int spriteCount = (spriteSheetData.spriteSheet.width / spriteSheetData.spriteSize.x) *
                          (spriteSheetData.spriteSheet.height / spriteSheetData.spriteSize.y);

        spriteSheetData.sprites = new SpriteRenderer[spriteCount];

        for (int y = 0; y < spriteSheetData.spriteSheet.height / spriteSheetData.spriteSize.y; y++)
        {
            for (int x = 0; x < spriteSheetData.spriteSheet.width / spriteSheetData.spriteSize.x; x++)
            {
                Rect spriteRect = new Rect(x * spriteSheetData.spriteSize.x, y * spriteSheetData.spriteSize.y, spriteSheetData.spriteSize.x, spriteSheetData.spriteSize.y);
                Vector2 pivot = new Vector2(0.5f, 0.5f);

                Sprite sprite = Sprite.Create(spriteSheetData.spriteSheet, spriteRect, pivot);
                int index = y * (spriteSheetData.spriteSheet.width / spriteSheetData.spriteSize.x) + x;

                GameObject spriteObject = new GameObject($"Sprite_{index}");
                SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteSheetData.sprites[index] = spriteRenderer;
            }
        }

        EditorUtility.SetDirty(spriteSheetData);
    }
}
