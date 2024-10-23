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
            if (boxCollider == null)
            {
                maskObject.SetActive(false);
                return;
            }
            if (!value)
            {
                boxCollider.size = renderer.size;
                boxCollider.offset = defaultOffset;
                maskObject.SetActive(false);
                return;
            }
            maskObject.SetActive(true);
        }
    }

    [SerializeField] private GameObject maskObject;
    private BoxCollider2D boxCollider;
    private Vector2 defaultOffset;
    private SpriteRenderer renderer;

    [Header("Position In Editor: Zero Energy")]
    [SerializeField] Vector2 posChangeForMaxEnergy;

    private void Awake()
    {
        initPos = transform.position;
        lastFull = this.GetPercentFull();

        boxCollider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();

        boxCollider.size = renderer.size;
        defaultOffset = boxCollider.offset;

        ShouldDisappear = shouldDisappear;
    }

    private float lastFull;

    /// <summary>
    /// Updates the door's position based on the amount of energy supplied to it.
    /// </summary>
    void Update()
    {
        float percentFull = this.GetPercentFull();

        if (!Mathf.Approximately(lastFull, percentFull))
        {

            transform.position = Vector2.Lerp(initPos, initPos + posChangeForMaxEnergy, percentFull);

            if (shouldDisappear)
            {
                maskObject.SetActive(true);
                boxCollider.size = new Vector2(renderer.size.x, renderer.size.y * (1 - percentFull));
                boxCollider.offset = defaultOffset + new Vector2(0, renderer.size.y * -(percentFull / 2));
            }
        }

        lastFull = percentFull;
    }


#if UNITY_EDITOR
    // switches disappear functionality when value is changed in editor
    private void OnValidate() => ShouldDisappear = shouldDisappear;
#endif
}
