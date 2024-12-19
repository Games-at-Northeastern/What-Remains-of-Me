using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class BossRotation : MonoBehaviour
{
    public GameObject player;
    public Vector3 _rotationOffset;
    public float Speed = 5;
    public Vector2 offSetPoint;


    private void Start()
    {
        offSetPoint = new Vector2(transform.position.x, transform.position.y);
    }
    private void Update()
    {
        /*
        if (player != null)
        {
            Vector3 initialTarget = player.transform.rotation.eulerAngles + _rotationOffset;
            initialTarget = new Vector3(0, 0, Mathf.Clamp(initialTarget.z, 18f, 28f));
            Vector3 target = new Vector3(0, 0, initialTarget.z);
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, target, Speed * Time.deltaTime);
        }
        
        if(intrigger)
            _rotationOffset bigger
                else
                _rotationOffset 0
            */
        if (player != null)
        {
            if (player.transform.position.x < offSetPoint.x)
                _rotationOffset = new Vector3(-3, 4, 0);
            else
                _rotationOffset = new Vector3(3, 4, 0);
        }
        if (player != null)
        {
            Vector2 initialTarget = ((player.transform.position+_rotationOffset) - transform.position).normalized;
            gameObject.transform.up = Vector2.Lerp(transform.up, initialTarget, Speed*Time.deltaTime);
        }
    }
}
