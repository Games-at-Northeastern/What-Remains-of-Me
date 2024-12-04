using System.Collections.Generic;
using UnityEngine;


/*
 *A class to work around unity's inability to serialize a dictionary. 
 * Converts dictionary data types into lists which can be converted to json through the JsonUtility class.
 */
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] protected List<TKey> keys = new List<TKey>();
    [SerializeField] protected List<TValue> values = new List<TValue>();

    public virtual void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }
    public virtual void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            //If your getting this error you might be assigning a dictionary a type that is too complex for this implementation
            Debug.LogError("Something went horribly wrong and the size of values doesnt equal the size of keys.");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }

    }

}
