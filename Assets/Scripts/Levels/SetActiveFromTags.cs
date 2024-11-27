using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveFromTags : MonoBehaviour
{
    /*[System.Serializable]
    private class TagActivasion
    {
        [SerializeField] private LevelManager.TagContainer enabledInVersions;
        [SerializeField] private List<GameObject> objects;

        public LevelManager.TagContainer EnabledInVersions { get => enabledInVersions; set => enabledInVersions = value; }
        public List<GameObject> Objects { get => objects; set => objects = value; }

        public TagActivasion()
        {
            enabledInVersions = new LevelManager.TagContainer();
            objects = new List<GameObject>();
        }
    }

    [SerializeField] private List<TagActivasion> requirements;

    public void Start()
    {
        LevelManager.TagContainer levelTags = FindObjectOfType<LevelManager>().LevelTags;

        foreach (TagActivasion requirement in requirements)
        {
            if (levelTags.HasFlags(requirement.EnabledInVersions))
            {
                continue;
            }

            foreach (GameObject gameObject in requirement.Objects)
            {
                gameObject.SetActive(false);
            }
        }
    }*/
}
