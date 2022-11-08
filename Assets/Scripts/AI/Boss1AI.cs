using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss1AI : MonoBehaviour
{

public enum FSMStates
    {
        Neutral, 
        Track,
        Attack,
        Overcharged
    }

    [SerializeField] GameObject player;
    public FSMStates currentState;

    public string name;

    private int _trackingTime;
    private int _attackingTime;
    private int _overchargedTime;
    private int _trackingSpeed;
    private bool _isChosen;



    // Start is called before the first frame update
    void Start()
    {

        currentState = FSMStates.Neutral;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        switch(currentState){

            case FSMStates.Neutral:
                UpdateNeutralState();
                break;
            case FSMStates.Track:
                UpdateTrackState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Overcharged:
                UpdateOverchargedState();
                break;

        }

    }


    void UpdateNeutralState(){

        print("Currently Neutral");

        if (_isChosen){

            currentState = FSMStates.Track;
        }
 
    }

    void UpdateTrackState(){

        print("Currently Tracking");

        FaceTarget(player.transform.position);
 
    }

    void UpdateAttackState(){

        print("Currently Attacking");
 
    }

    void UpdateOverchargedState(){

        print("Currently Overcharged");
 
    }

    void FaceTarget(Vector3 target){

        Vector3 distanceBetween = target - transform.position;
        float angle = Mathf.Atan2(distanceBetween.y, distanceBetween.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));

    }

    public void Chosen(){

        _isChosen = true;
    }





}
