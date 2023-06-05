using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlitchyText : MonoBehaviour
{
    [Header("Configs")]
    public float speed = 1f;
    public float amplitude = 1f;
    public float distance = 1f;

    private TMP_Text textComponent;

    private TMP_TextInfo textInfo;

    private Mesh mesh;
    private Vector3[] vertices;


    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // ensures text mesh is up to date
        textComponent.ForceMeshUpdate();
        textInfo = textComponent.textInfo;

        mesh = textComponent.mesh;
        vertices = mesh.vertices;

        GlitchEffect();
    }

    private void GlitchEffect()
    {
        // commented out code is another functionality implementation, try both out to see which one looks better

        /*
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                // creates wobble effect (up and down)
                //verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(((Time.time * speed) + orig.x) * amplitude) * distance, 0);
                // creates side to side shake effect
                verts[charInfo.vertexIndex + j] = orig + new Vector3(Mathf.Sin(((Time.time * speed) + orig.y) * amplitude) * distance, 0, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
        */

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = GetNewPosition(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textComponent.canvasRenderer.SetMesh(mesh);
    }

    /// <summary>
    /// Returns a new position for each vertex of each character in the text to create effect
    /// </summary>
    /// <param name="time"></param>
    /// <returns>Vector2</returns>
    Vector2 GetNewPosition(float time)
    {
        return new Vector2(Mathf.Sin(time * amplitude) * speed, Mathf.Cos(time * distance) * speed);
    }
}
