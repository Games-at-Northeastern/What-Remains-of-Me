using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartScriptableObjects.ReactiveProperties;

public class SetPlayerEnergyLevelOnStart : MonoBehaviour
{

    [SerializeField] private PercentageFloatReactivePropertySO playerBatterySO;


    void Start()
    {

        playerBatterySO.Value = 1f;
        
    }



}
