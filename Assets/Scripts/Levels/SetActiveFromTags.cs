using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveFromTags : MonoBehaviour
{
    [System.Serializable]
    private class TagActivasion
    {
        [SerializeField] private bool active = true;
        [SerializeField] private bool checkFail = false;
        public bool CheckFail => checkFail;
        public bool Active => active;
        [SerializeField] private bool greaterTrueMatchFalse = true;
        public bool GreaterTrueMatchFalse => greaterTrueMatchFalse;
        [SerializeField] private SerializableStringIntDict tagCase;
        [SerializeField] private List<GameObject> objects;

        public SerializableStringIntDict TagCase{ get => tagCase; set => tagCase = value; }
        public List<GameObject> Objects { get => objects; set => objects = value; }

        public TagActivasion() =>
            objects = new List<GameObject>();
    }

    [SerializeField] private List<TagActivasion> requirements;

    public void Start()
    {
        var levelTags = LevelManager.Tags;

        foreach (var requirement in requirements)
        {
            var succeed = true;
            foreach (var (tag, count) in requirement.TagCase)
            {
                if (requirement.GreaterTrueMatchFalse)
                {
                    if (!levelTags.HasGreaterThanOrEqual(tag, count))
                    {
                        succeed = false;
                        break;
                    }
                }
                else
                {
                    if (!levelTags.HasExact(tag, count))
                    {
                        succeed = false;
                        break;
                    }
                }
            }

            if (succeed != requirement.CheckFail)
            {
                foreach (GameObject gameObject in requirement.Objects)
                {
                    gameObject.SetActive(requirement.Active);
                }
            }
        }
    }
}
