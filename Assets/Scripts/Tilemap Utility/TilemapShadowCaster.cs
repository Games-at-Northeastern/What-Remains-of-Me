using System;
using System.Reflection;

namespace UnityEngine.Rendering.Universal
{
    public class TilemapShadowCaster : ShadowCaster2D
    {
        private static BindingFlags accessFlagsPrivate =
            BindingFlags.NonPublic | BindingFlags.Instance;

        private static FieldInfo shapePathField =
            typeof(ShadowCaster2D).GetField("m_ShapePath", accessFlagsPrivate);

        private static FieldInfo meshHashField =
            typeof(ShadowCaster2D).GetField("m_ShapePathHash", accessFlagsPrivate);

        [SerializeField] private CompositeCollider2D tilemapCollider;

        public void SetPath(Vector2[] path)
        {
            Vector3[] converted = Array.ConvertAll<Vector2, Vector3>(path, input => input);

            selfShadows = true;
            shapePathField.SetValue(this, converted);
            meshHashField.SetValue(this, -1);
        }
    }
}
