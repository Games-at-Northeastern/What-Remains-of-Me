using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetVirusShaderLevel : MonoBehaviour
{
    [SerializeField] private float _virusLevel;

    [SerializeField] private Image _batteryUIBar;

    private Material _batteryUIBarMaterial;

    private void Start()
    {
        _batteryUIBarMaterial = _batteryUIBar.GetComponent<Material>();
    }

    private void Update()
    {
        _batteryUIBar.material = _batteryUIBarMaterial;
        _batteryUIBarMaterial.SetFloat("_Fade_Amount", _virusLevel);
    }
}
