using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartScriptableObjects.ReactiveProperties;


public class UpdateVirusUIVertical : MonoBehaviour
{

    [SerializeField] private PercentageFloatReactivePropertySO virusPercent;
    [SerializeField] private RectTransform virusTransform;
    //[SerializeField] private float _maxMaskWidth;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        setCurrentVirusPercentage(virusPercent.Value);
    }

    //
    public void setCurrentVirusPercentage(float percentage)
    {
        virusTransform.sizeDelta = new Vector2(virusTransform.sizeDelta.x, (int)Mathf.CeilToInt(percentage * 100));
    }
}
