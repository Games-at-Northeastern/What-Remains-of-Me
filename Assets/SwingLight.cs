using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SwingLight : MonoBehaviour
{
    [SerializeField]
    private float swingIntensity = 5f;

    [SerializeField]
    private float swingAmount = 2f;

    private Quaternion leftRotation;
    private Quaternion rightRotation;


    void Start()
    {
        var baseRotation = transform.rotation;
        leftRotation = baseRotation * Quaternion.Euler(0, 0, -swingAmount);
        rightRotation = baseRotation * Quaternion.Euler(0, 0, swingAmount);
    }


    void Update()
    {
       
        float time = (Mathf.Sin(Time.deltaTime * swingIntensity) + 1f) / 2f;

        transform.rotation = Quaternion.Slerp(leftRotation, rightRotation, time);
    }
}
