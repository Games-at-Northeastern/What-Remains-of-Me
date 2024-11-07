using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections;

[CreateAssetMenu(fileName = "Connected Rule Tile", menuName = "2D/Tiles/Connected Rule Tile")]
public class ConnectedRuleTile : RuleTile
{
    [SerializeField] private SerializableTileBaseHashSet linkedTiles = new();

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other == null)
        {
            return base.RuleMatch(neighbor, other);
        }

        switch (neighbor)
        {
            case TilingRuleOutput.Neighbor.This:
                return linkedTiles.Contains(other) || other == this;
            case TilingRuleOutput.Neighbor.NotThis:
                return !linkedTiles.Contains(other) && other != this;
            default:
                break;
        }
        return true;

    }

    [System.Serializable]
    public class SerializableTileBaseHashSet :
    ISerializationCallbackReceiver,
    ISet<TileBase>,
    IReadOnlyCollection<TileBase>
    {
        [SerializeField] private List<TileBase> values;
        [System.NonSerialized] private HashSet<TileBase> _hashSet;

        #region Constructors

        // empty constructor required for Unity serialization
        public SerializableTileBaseHashSet()
        {
            values = new List<TileBase>();
            _hashSet = new HashSet<TileBase>();
        }

        public SerializableTileBaseHashSet(IEnumerable<TileBase> collection)
        {
            _hashSet = new HashSet<TileBase>(collection);
        }

        #endregion Constructors


        #region Interface forwarding to the _hashset

        public int Count => _hashSet.Count;
        public bool IsReadOnly => false;
        //public bool ISet<TileBase>.Add(TileBase item) => _hashSet.Add(item);
        public bool Add(TileBase item) => _hashSet.Add(item);
        void ICollection<TileBase>.Add(TileBase item) => _hashSet.Add(item);
        public bool Remove(TileBase item) => _hashSet.Remove(item);
        public void ExceptWith(IEnumerable<TileBase> other) => _hashSet.ExceptWith(other);
        public void IntersectWith(IEnumerable<TileBase> other) => _hashSet.IntersectWith(other);
        public bool IsProperSubsetOf(IEnumerable<TileBase> other) => _hashSet.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<TileBase> other) => _hashSet.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<TileBase> other) => _hashSet.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<TileBase> other) => _hashSet.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<TileBase> other) => _hashSet.Overlaps(other);
        public bool SetEquals(IEnumerable<TileBase> other) => _hashSet.SetEquals(other);
        public void SymmetricExceptWith(IEnumerable<TileBase> other) => _hashSet.SymmetricExceptWith(other);
        public void UnionWith(IEnumerable<TileBase> other) => _hashSet.UnionWith(other);
        public void Clear() => _hashSet.Clear();
        public bool Contains(TileBase item) => _hashSet.Contains(item);
        public void CopyTo(TileBase[] array, int arrayIndex) => _hashSet.CopyTo(array, arrayIndex);

        public IEnumerator<TileBase> GetEnumerator() => _hashSet.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion Interface forwarding to the _hashset


        #region ISerializationCallbackReceiver implemenation

        public void OnBeforeSerialize()
        {
            /*var cur = new HashSet<TileBase>(values);

            foreach (var val in this)
            {
                if (!cur.Contains(val))
                {
                    values.Add(val);
                }
            }*/

            OnAfterDeserialize();
        }

        public void OnAfterDeserialize()
        {
            if (values.Count == Count)
            {
                return;
            }

            Clear();

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] is null)
                {
                    continue;
                }

                if (Contains(values[i]))
                {
                    values[i] = null;
                    continue;
                }

                Add(values[i]);
            }
        }

        #endregion ISerializationCallbackReceiver implemenation

#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(SerializableTileBaseHashSet))]
        private class SerializableHashSetPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var internalListProp = property.FindPropertyRelative("values");

                EditorGUILayout.LabelField(label);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                bool add = GUILayout.Button("Add");
                bool remove = GUILayout.Button("Remove");

                for (int i = 0; i < internalListProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(internalListProp.GetArrayElementAtIndex(i));
                }

                if (add)
                {
                    internalListProp.InsertArrayElementAtIndex(internalListProp.arraySize);
                    internalListProp.GetArrayElementAtIndex(internalListProp.arraySize - 1).objectReferenceValue = null;
                }

                if (remove && internalListProp.arraySize > 0)
                {
                    internalListProp.DeleteArrayElementAtIndex(internalListProp.arraySize - 1);
                }

                EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}
