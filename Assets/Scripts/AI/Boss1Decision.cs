using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Decision : MonoBehaviour
{

    [SerializeField] Boss1AI[] machines;


    // Start is called before the first frame update
    void Start()
    {

        randomizedDecision();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void randomizedDecision(){
        int index = Random.Range(0, machines.Length);
        Boss1AI selectedMachine = machines[index];
        print(selectedMachine.name + " is chosen by K.E.N.K");
        selectedMachine.Chosen();
       
    }


    
}
