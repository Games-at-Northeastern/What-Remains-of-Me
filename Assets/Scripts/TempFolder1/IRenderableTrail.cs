using UnityEngine;

/// <summary>
/// An interface exposing a method for baking trail points to a script.
/// Primarily used with LineRendererUtility.
/// </summary>
public interface IRenderableTrail
{
    /// <summary>
    /// Set this gameobject's important points to the input.
    /// </summary>
    /// <param name="points"></param>
    void SetPoints(Transform[] points);

    // hacky solution
#if UNITY_EDITOR
    /// <summary>
    /// A helper for when rendered trails are created in the editor and need
    /// to be saved into trail objects.
    /// </summary>
    /// <param name="points"></param>
    void EditorOnlySetPoints(Transform[] points);
#endif
}
