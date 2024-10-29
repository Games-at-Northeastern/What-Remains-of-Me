using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Represents a door which can be moved up or down based on the amount of energy
/// supplied to it.
/// </summary>
///

public class ControllableDoor : AControllable
{
    Vector2 initPos;

    [Header("Should door dissapear as it opens?")]
    [SerializeField] private bool shouldDisappear = false;

    public bool ShouldDisappear
    {
        get => shouldDisappear;
        set
        {
            shouldDisappear = value;
            maskObject.SetActive(value);
            if (!boxCollider || !doorRenderer)
            {
                return;
            }

            RecalcMaskLoc();

            if (value)
            {
                InvertMask = invertMask;
            }
            if (!value)
            {
                doorRenderer.maskInteraction = SpriteMaskInteraction.None;
                boxCollider.size = doorRenderer.size;
                boxCollider.offset = defaultOffset;
                return;
            }
        }
    }

    [System.NonSerialized]
    public bool overrideEnergy = false;
    [System.NonSerialized]
    public float overridePercentFull = 0;

    [SerializeField] private GameObject maskObject;

    [Tooltip("Sets object to be fully visible at 100% energy, rather than at 0% energy. Enable when making a door which opens as energy is drained, rather than as energy is inputted. ")]
    [SerializeField] private bool invertMask;

    public bool InvertMask
    {
        get => invertMask;
        set
        {
            invertMask = value;

            if (shouldDisappear && doorRenderer)
            {
                RecalcMaskLoc();
                if (!invertMask)
                {
                    doorRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    doorRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
            }
        }
    }

    private BoxCollider2D boxCollider = null;
    private Vector2 defaultOffset;
    private SpriteRenderer doorRenderer = null;

    [Header("Position In Editor: Zero Energy")]
    [SerializeField] private Vector2 posChangeForMaxEnergy;

    private void Awake()
    {
        initPos = transform.position;
        lastFull = -1;

        boxCollider = GetComponent<BoxCollider2D>();
        doorRenderer = GetComponent<SpriteRenderer>();

        boxCollider.size = doorRenderer.size;
        defaultOffset = boxCollider.offset;

        maskObject.transform.localScale = doorRenderer.size;

        ShouldDisappear = shouldDisappear;
    }

    private float lastFull;

    /// <summary>
    /// Updates the door's position based on the amount of energy supplied to it.
    /// </summary>
    private new void Update()
    {
        base.Update();
        float percentFull = this.GetPercentFull();

        if (overrideEnergy)
        {
            percentFull = overridePercentFull;
        }

        if (!Mathf.Approximately(lastFull, percentFull))
        {

            transform.position = Vector2.Lerp(initPos, initPos + posChangeForMaxEnergy, percentFull);

            if (shouldDisappear)
            {
                maskObject.SetActive(true);
                // boxCollider.size = new Vector2(doorRenderer.size.x, doorRenderer.size.y * (1 - percentFull));
                //boxCollider.offset = defaultOffset + new Vector2(0, doorRenderer.size.y * -(percentFull / 2));
                CalcColliderSize();
            }
        }

        lastFull = percentFull;
    }

    private void CalcColliderSize()
    {
        SpriteMask maskRend = maskObject.GetComponent<SpriteMask>();

        if (posChangeForMaxEnergy.y != 0)
        {
            float lowBound = 0;
            float highBound = 0;
            float shiftSign = 0;

            if (posChangeForMaxEnergy.y * (invertMask ? -1 : 1) < 0)
            {
                lowBound = doorRenderer.bounds.center.y - doorRenderer.bounds.extents.y;
                highBound = maskRend.bounds.center.y + maskRend.bounds.extents.y;
                shiftSign = 1;
            }

            if (posChangeForMaxEnergy.y * (invertMask ? -1 : 1) > 0)
            {
                lowBound = maskRend.bounds.center.y - maskRend.bounds.extents.y;
                highBound = doorRenderer.bounds.center.y + doorRenderer.bounds.extents.y;
                shiftSign = -1;
            }

            if (lowBound >= highBound)
            {
                boxCollider.size = doorRenderer.size;
                boxCollider.offset = defaultOffset;
                return;
            }
            else
            {
                float dist = highBound - lowBound;
                float percentDec = dist / doorRenderer.bounds.size.y;
                boxCollider.size = new Vector2(doorRenderer.size.x, doorRenderer.size.y * (1 - percentDec));
                boxCollider.offset = new Vector2(0, doorRenderer.size.y * (shiftSign * percentDec / 2));
                return;
            }
        }

        if (posChangeForMaxEnergy.x != 0)
        {
            float lowBound = 0;
            float highBound = 0;
            float shiftSign = 0;

            if (posChangeForMaxEnergy.x * (invertMask ? -1 : 1) < 0)
            {
                lowBound = doorRenderer.bounds.center.x - doorRenderer.bounds.extents.x;
                highBound = maskRend.bounds.center.x + maskRend.bounds.extents.x;
                shiftSign = 1;
            }

            if (posChangeForMaxEnergy.x * (invertMask ? -1 : 1) > 0)
            {
                lowBound = maskRend.bounds.center.x - maskRend.bounds.extents.x;
                highBound = doorRenderer.bounds.center.x + doorRenderer.bounds.extents.x;
                shiftSign = -1;
            }

            if (lowBound >= highBound)
            {
                boxCollider.size = doorRenderer.size;
                boxCollider.offset = defaultOffset;
                return;
            }
            else
            {
                float dist = highBound - lowBound;
                float percentDec = dist / doorRenderer.bounds.size.x;
                boxCollider.size = new Vector2(doorRenderer.size.x * (1 - percentDec), doorRenderer.size.y);
                boxCollider.offset = new Vector2(doorRenderer.size.x * (shiftSign * percentDec / 2), 0);
                return;
            }
        }
    }

    private void RecalcMaskLoc()
    {
        if (invertMask)
        {
            maskObject.transform.position = initPos;
            return;
        }
        float maskOffX = Mathf.Abs(posChangeForMaxEnergy.x);
        float maskOffSign = Mathf.Sign(posChangeForMaxEnergy.x);
        if (maskOffX > 0)
        {
            maskOffX = Mathf.Max(posChangeForMaxEnergy.x, doorRenderer.size.x);
        }
        maskOffX *= maskOffSign;

        float maskOffY = Mathf.Abs(posChangeForMaxEnergy.y);
        maskOffSign = Mathf.Sign(posChangeForMaxEnergy.y);
        if (maskOffY > 0)
        {
            maskOffY = Mathf.Max(posChangeForMaxEnergy.y, doorRenderer.size.y);
        }
        maskOffY *= maskOffSign;

        maskObject.transform.position = initPos + new Vector2(maskOffX, maskOffY);
    }

#if UNITY_EDITOR
    // switches disappear and invertmask functionality when value is changed in editor
    private void OnValidate()
    {
        base.OnValidate();
        ShouldDisappear = shouldDisappear;
        InvertMask = invertMask;
    }

    [CustomEditor(typeof(ControllableDoor))]
    public class ControllableDoorEditor : AControllable_Editor
    {
        private void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, this.propertiesInBaseClass);

            base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
