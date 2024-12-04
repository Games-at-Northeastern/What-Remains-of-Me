using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
public class LevelTagData : ScriptableObject
{
    public List<LevelTagSO> Tags;
    public SerializableDictionary<Scene, LevelTagDictionary> SceneAcceptTags;
}
