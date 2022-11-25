using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDataResetter : MonoBehaviour
{
    [SerializeField] private DatabaseInt[] _intVariablesToReset;
    [SerializeField] private DatabaseFloat[] _floatVariablesToReset;
    //[SerializeField] private BoolVariable[] _boolVariablesToReset;
    [SerializeField] private DataBaseRecord[] _stringVariablesToReset;

    private void Awake()
    {
        foreach (DatabaseInt intVariable in _intVariablesToReset)
        {
            intVariable.IntValue = 0;
        }
        foreach (DatabaseFloat floatVariable in _floatVariablesToReset)
        {
            floatVariable.FloatValue = 0.0f;
        }
        // foreach (BoolVariable boolVariable in _boolVariablesToReset)
        // {
        //     boolVariable.Value = false;
        // }
        foreach (DataBaseRecord stringVariable in _stringVariablesToReset)
        {
            stringVariable.Value = "";
        }
    }
}
