using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    [SerializeField] private List<GameObject> doorParts;

    [SerializeField] private bool open = false;
    public bool IsOpen => open;

    private Dictionary<SpringJoint2D, float> springJoints;

    private void Awake()
    {
        springJoints = new Dictionary<SpringJoint2D, float>();

        foreach (var gameObject in doorParts)
        {
            var joint2D = gameObject.GetComponent<SpringJoint2D>();

            if (joint2D == null)
            {
                continue;
            }

            springJoints.Add(joint2D, joint2D.distance);
        }
    }

    private void Start()
    {
        if (open)
        {
            DoOpen();
            return;
        }

        DoClose();
    }

    public void Switch()
    {
        if (open)
        {
            Close();
            return;
        }

        Open();
    }

    public bool Open()
    {
        if (open)
        {
            return false;
        }

        DoOpen();
        open = true;
        return true;
    }

    private void DoOpen()
    {
        foreach ((SpringJoint2D joint2D, _) in springJoints)
        {
            if (joint2D != null)
            {
                joint2D.distance = 0f;
                joint2D.dampingRatio = .2f;
                joint2D.frequency = 2.5f;
                joint2D.gameObject.GetComponent<Rigidbody2D>().WakeUp();
            }
        }
    }

    public bool Close()
    {
        if (!open)
        {
            return false;
        }

        DoClose();
        open = false;
        return true;
    }

    private void DoClose()
    {
        foreach ((SpringJoint2D joint2D, float distance) in springJoints)
        {
            if (joint2D != null)
            {
                joint2D.distance = distance;
                joint2D.dampingRatio = 0.5f;
                joint2D.frequency = 3;
                joint2D.gameObject.GetComponent<Rigidbody2D>().WakeUp();
            }
        }
    }

    public void OpenClose() => StartCoroutine(DoOpenClose());

    private IEnumerator DoOpenClose()
    {
        Open();

        yield return new WaitForSeconds(1);

        Close();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Trapdoor))]
public class TrapdoorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        // open close button

        Trapdoor trapdoor = serializedObject.targetObject as Trapdoor;

        if (GUILayout.Button(trapdoor.IsOpen ? "Close" : "Open"))
        {
            trapdoor.Switch();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
