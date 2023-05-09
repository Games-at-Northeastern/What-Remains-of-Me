using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : AControllable
{
    [SerializeField] Boss1AI machine;
    public SpriteRenderer body;
    public GameObject water;
    public float activeTimer {get; set;}
    private int _activeTime = 2;
    bool _activated;


    // Start is called before the first frame update
    void Awake()
    {
       _activated = false;
    }

    // Update is called once per frame
    void Update()
    {

        if ((energy + virus) == maxCharge && !_activated)
        {
            body.color = Color.blue;
            print("Valve fully charged");
            water.SetActive(true);
            _activated = true;
            activeTimer = _activeTime;
            machine._hitWithValve = true;
            machine.enabled = false;
        }

       if(activeTimer > 0.0f)
    {
        activeTimer -= Time.deltaTime;
        if(activeTimer <= 0)
        {
            print("valve reset");
            body.color = Color.white;
            water.SetActive(false);
            _activated = false;
            energy = 0;
        }
    }


    }

}
