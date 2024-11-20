using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequireLevelVersion : MonoBehaviour
{
    [System.Serializable]
    public class VersionRequirement
    {
        [SerializeField] private List<int> enabledInVersions;
        [SerializeField] private List<GameObject> objects;

        public List<int> EnabledInVersions { get => enabledInVersions; set => enabledInVersions = value; }
        public List<GameObject> Objects { get => objects; set => objects = value; }

        public VersionRequirement()
        {
            enabledInVersions = new List<int>();
            objects = new List<GameObject>();
        }
    }

    [SerializeField] private List<VersionRequirement> requirements;

    public void Start()
    {
        int levelVersion = FindObjectOfType<LevelManager>().LevelVersion;

        foreach (VersionRequirement requirement in requirements)
        {
            if (requirement.EnabledInVersions.Contains(levelVersion))
            {
                continue;
            }

            foreach (GameObject gameObject in requirement.Objects)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
