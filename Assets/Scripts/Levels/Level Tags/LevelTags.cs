using System.Collections.Generic;
using UnityEngine;

public class LevelTagData : ScriptableObject
{
    public List<LevelTagSO> Tags { get; private set; }
}

public class LevelTagSO : ScriptableObject
{
    public string Path { get; set; }
}
