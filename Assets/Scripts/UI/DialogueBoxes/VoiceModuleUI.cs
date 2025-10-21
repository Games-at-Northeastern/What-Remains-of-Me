using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

public class VoiceModuleUI : MonoBehaviour
{
    public static VoiceModuleUI Instance { get; private set; }
    public List<VoiceModuleOption> optionPrefabs;
    private List<VoiceModuleOption> childTransforms = new();
    public float timeForRotate = .5f;
    private float currentRotateTime;
    int currentRotation = 0;
    [SerializeField] private GameObject WheelUI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        currentRotateTime = timeForRotate;
        Instance = this;
    }

    public VoiceModule.VoiceTypes Rotate()
    {
        if(timeForRotate == currentRotateTime)
        {
            currentRotateTime = 0;
        }
        return childTransforms[(currentRotation + 1) % childTransforms.Count].type;
    }
    public void ForceRotateTo(VoiceModule.VoiceTypes type)
    {
        currentRotateTime = 0;
        for(int x =0; x < childTransforms.Count; x++)
        {
            if (childTransforms[x].type == type)
            {
                currentRotation = (x - 1) % childTransforms.Count;
            }
        }
    }

    private float CurrentAngle(float rangeOfRotation) => -1 * ((currentRotation * rangeOfRotation) + currentRotateTime / timeForRotate * rangeOfRotation);

    private void CalculateAngles()
    {

        float rangeOfRotation = 360.0f / childTransforms.Count;
        float angle = CurrentAngle(rangeOfRotation);
        WheelUI.transform.rotation = Quaternion.Euler(0, 0, angle);
        foreach (VoiceModuleOption voiceModule in childTransforms)
        {
            voiceModule.transform.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    private void Update()
    {
        if (!WheelUI.activeSelf)
        {
            return;
        }

        if (timeForRotate != currentRotateTime && childTransforms.Count > 0)
        {
            CalculateAngles();
            //Range of how many degrees each rotation will take
            currentRotateTime = Mathf.Min(currentRotateTime + Time.deltaTime, timeForRotate);
            if (currentRotateTime == timeForRotate)
            {
                CalculateAngles();
                currentRotation++;
                if (currentRotation == childTransforms.Count)
                {
                    currentRotation = 0;
                }
            }
        }
    }

    public void InitiateOptions(List<VoiceModule.VoiceTypes> types, int current)
    {
        int moduleCount = 0;
        currentRotation = current;
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(true);
        }
        foreach (var prefab in optionPrefabs)
        {
            if(types.Contains(prefab.type))
            {
                var standardPoint = new Vector3(0, 30, 0);
                var rotation = Quaternion.AngleAxis(moduleCount / (float)types.Count() * 360f, Vector3.forward);
                var rotatedRelativePosition = rotation * standardPoint;
                var newObj = Instantiate(prefab.gameObject, Vector3.zero, Quaternion.identity, WheelUI.transform);
                newObj.transform.localPosition = rotatedRelativePosition;
                childTransforms.Add(newObj.GetComponent<VoiceModuleOption>());
                moduleCount++;
            }
        }
    }

}
