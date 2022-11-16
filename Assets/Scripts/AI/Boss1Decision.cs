using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Decision : MonoBehaviour
{

    [SerializeField] Boss1AI[] machines;

    public float timer { get; set; }

    private int _decisionTime = 20;


    // Start is called before the first frame update
    void Start()
    {

        randomizedDecision();
        
    }

    // Update is called once per frame
    void Update()
    {

        if(timer > 0.0f)
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            randomizedDecision();
        }
    }

    }

    void randomizedDecision(){
        int index = Random.Range(0, machines.Length);
        Boss1AI selectedMachine = machines[index];
        selectedMachine.Chosen();
        timer = _decisionTime;
       
    }


    
}
