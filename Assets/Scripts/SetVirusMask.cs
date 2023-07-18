namespace UI.PlayerBatteryMeter
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class SetVirusMask : MonoBehaviour
    {

        [SerializeField] private RectTransform _maskRectTransform;
        [SerializeField] private float _maxMaskWidth;


        [SerializeField] private PlayerInfo AtlasPlayerInfo;

        private void Update()
        {

            _maskRectTransform.sizeDelta = new Vector2(AtlasPlayerInfo.virus/2.62f, _maskRectTransform.sizeDelta.y);
            Debug.Log(AtlasPlayerInfo.virus);
        }


    }


}
