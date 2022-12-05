using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Decision : MonoBehaviour
{

    [SerializeField] Boss1AI[] machines;
    [SerializeField] int _decisionTime = 20;

    public float timer { get; set; }

    
    



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

        List<Boss1AI> possibleMachines = new List<Boss1AI>();

        foreach(Boss1AI m in machines){
            if(!m._isChosen){
                possibleMachines.Add(m);
            }

        }
        int index = Random.Range(0, possibleMachines.Count);
        Boss1AI selectedMachine = possibleMachines[index];
        selectedMachine.Chosen();
        timer = _decisionTime;
       
    }


    
}
