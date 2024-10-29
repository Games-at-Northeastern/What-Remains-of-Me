using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Abstract class which gives Controllable classes certain utilities,
/// such as a general implementation of the GainEnergy() and LoseEnergy()
/// methods, and a representation of energy.
/// </summary>
public abstract class AControllable : MonoBehaviour
{
    public PlayerInfo playerInfo;

    // Event to be triggered whenever the virus amount changes,
    // sending the current percentage of energy that is virus as a value between 0 and 1
    [NonSerialized] public UnityEvent<float> OnVirusChange;
    public void VirusChange(float virusPercentage) => OnVirusChange?.Invoke(virusPercentage);

    [NonSerialized] public UnityEvent<float> OnEnergyChange;
    public void EnergyChange(float totalEnergyAmount) => OnEnergyChange?.Invoke(totalEnergyAmount);

    [SerializeField] protected List<Outlet> outlets;

    public enum OutletCombinationType
    {
        Arithmetic,
        Highest,
        Lowest
    }

    public enum ArithComboOperation
    {
        Add,
        Subtract,
        None

    }

    [Serializable]
    public class OutletArithmeticCombination
    {
        public ArithComboOperation energyOperation = ArithComboOperation.Add;
        public ArithComboOperation maxOperation = ArithComboOperation.Add;
    }

    [SerializeField] protected OutletCombinationType outletCombinationType;
    [SerializeField] protected List<OutletArithmeticCombination> outletArithmeticCombinations;

    protected void Start() => StoreEnergyVals();

    protected void Update()
    {
        if (CheckEnergyVals())
        {
            StoreEnergyVals();
        }
    }

    // returns the total energy supplied by outlets
    public float GetEnergy() => GetEnergyQuantity("total");

    // returns the total clean energy supplied by outlets
    public float GetClean() => GetEnergyQuantity("clean");

    // returns the total virus energy supplied by outlets
    public float GetVirus() => GetEnergyQuantity("virus");

    // returns the percentage of energy supplied by the outlets out of the total possible energy supplied by the outlets
    public float GetPercentFull() => Mathf.Clamp(GetEnergyQuantity("total") / GetEnergyMax(), 0, 1);

    // returns what percent of energy in the outlet is clean
    public float? GetPercentClean()
    {
        float total = GetEnergyQuantity("total");
        if (total == 0)
        {
            return null;
        }

        return Mathf.Clamp(GetEnergyQuantity("clean") / total, 0, 1);
    }

    // returns what percent of energy in the outlet is virus
    public float? GetPercentVirus()
    {
        float total = GetEnergyQuantity("total");
        if (total == 0)
        {
            return null;
        }

        return Mathf.Clamp(GetEnergyQuantity("virus") / total, 0, 1);
    }

    /// <summary>
    /// Returns the total energy of some type this controllable has supplied by outlets
    /// </summary>
    private float GetEnergyQuantity(string energyType)
    {
        Func<Outlet, float> eGetter = (outlet) => 0;

        switch (energyType)
        {
            case "clean":
                eGetter = (outlet) => outlet.GetClean();
                break;
            case "virus":
                eGetter = (outlet) => outlet.GetVirus();
                break;
            case "total":
                eGetter = (outlet) => outlet.GetEnergy();
                break;
            default:
                return 0;
        }

        if (outlets.Count == 0)
        {
            return 0;
        }

        if (outlets.Count == 1)
        {
            return eGetter.Invoke(outlets[0]);
        }

        switch (outletCombinationType)
        {
            case OutletCombinationType.Arithmetic:
                float energy = 0;
                for (int i = 0; i < outlets.Count; i++)
                {
                    float outVal = eGetter(outlets[i]);
                    energy += GetArithmeticChange(outVal, outletArithmeticCombinations[i].energyOperation);
                }
                return energy;
            case OutletCombinationType.Highest:
                float highEnergy = 0;
                for (int i = 0; i < outlets.Count; i++)
                {
                    float energ = eGetter(outlets[i]);
                    if (energ > highEnergy)
                    {
                        highEnergy = energ;
                    }
                }
                return highEnergy;
            case OutletCombinationType.Lowest:
                float lowEnergy = float.MaxValue;
                bool changed = false;
                for (int i = 0; i < outlets.Count; i++)
                {
                    float energ = eGetter(outlets[i]);
                    if (energ < lowEnergy)
                    {
                        lowEnergy = energ;
                        changed = true;
                    }
                }
                return lowEnergy * (changed ? 1 : 0);
            default:
                return 0;
        }
    }

    public float GetEnergyMax()
    {
        if (outlets.Count == 0)
        {
            return 0;
        }

        if (outlets.Count == 1)
        {
            return outlets[0].GetMaxEnergy();
        }

        switch (outletCombinationType)
        {
            case OutletCombinationType.Arithmetic:
                float energyMax = 0;
                for (int i = 0; i < outlets.Count; i++)
                {
                    energyMax += GetArithmeticChange(outlets[i].GetMaxEnergy(), outletArithmeticCombinations[i].maxOperation);
                }
                return energyMax;
            case OutletCombinationType.Lowest:
            case OutletCombinationType.Highest:
                float highMax = 0;
                for (int i = 0; i < outlets.Count; i++)
                {
                    float energ = outlets[i].GetMaxEnergy();
                    if (energ > highMax)
                    {
                        highMax = energ;
                    }
                }
                return highMax;
            default:
                return 0;
        }
    }


    private float GetArithmeticChange(float val, ArithComboOperation combo) => combo switch
    {
        ArithComboOperation.Add => val,
        ArithComboOperation.Subtract => -val,
        ArithComboOperation.None => 0,
        _ => 0,
    };

    public int GetNumOutlets() => outlets.Count;

    // private methods for checking changes in energy

    private float previousEnergy = 0;
    private float previousVirus = 0;
    private float? previousVirusPercent = 0;

    private void StoreEnergyVals()
    {
        previousEnergy = GetEnergy();
        previousVirus = GetVirus();
        previousVirusPercent = GetPercentVirus();
    }

    private bool CheckEnergyVals()
    {
        bool res = false;

        if (previousEnergy != GetEnergy())
        {
            EnergyChange(GetEnergy());
            res = true;
        }

        if (previousVirus != GetVirus() || previousVirusPercent != GetPercentVirus())
        {
            VirusChange(GetVirus());
            res = true;
        }

        return res;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AControllable), false)]
    public class AControllable_Editor : Editor
    {
        private bool baseFoldout = false;

        private SerializedProperty outletsProp;
        private SerializedProperty outletCombinationTypeProp;
        private SerializedProperty outletArithmeticCombinationsProp;
        private SerializedProperty playerInfoProp;

        protected string[] propertiesInBaseClass = new string[] { "outlets", "outletCombinationType", "outletArithmeticCombinations", "playerInfo"};

        protected void OnEnable()
        {
            outletsProp = serializedObject.FindProperty(propertiesInBaseClass[0]);
            outletCombinationTypeProp = serializedObject.FindProperty(propertiesInBaseClass[1]);
            outletArithmeticCombinationsProp = serializedObject.FindProperty(propertiesInBaseClass[2]);
            playerInfoProp = serializedObject.FindProperty(propertiesInBaseClass[3]);
        }
        public override void OnInspectorGUI()
        {
            //serializedObject.Update();

            AControllable controllable = target as AControllable;

            EditorGUILayout.Space();

            baseFoldout = EditorGUILayout.Foldout(baseFoldout, "AControllable Settings");

            if (baseFoldout)
            {

                EditorGUILayout.PropertyField(playerInfoProp);

                EditorGUILayout.PropertyField(outletsProp);

                if (controllable.outlets.Count > 1)
                {
                    EditorGUILayout.PropertyField(outletCombinationTypeProp);

                    if (controllable.outletCombinationType == OutletCombinationType.Arithmetic)
                    {
                        EditorGUILayout.PropertyField(outletArithmeticCombinationsProp);
                    }
                }
            }

            //serializedObject.ApplyModifiedProperties();
        }
    }

    protected void OnValidate()
    {
        if (outlets.Count > 1 && outletCombinationType == OutletCombinationType.Arithmetic)
        {
            while (outletArithmeticCombinations.Count > outlets.Count)
            {
                outletArithmeticCombinations.RemoveAt(outletArithmeticCombinations.Count + (1 * -1));
            }
            while (outletArithmeticCombinations.Count < outlets.Count)
            {
                outletArithmeticCombinations.Add(new OutletArithmeticCombination());
            }
        }
    }
#endif
}
