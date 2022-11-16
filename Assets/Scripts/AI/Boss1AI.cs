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

    public SpriteRenderer body;

    private int _trackingTime = 5;
    private int _attackingTime = 5;
    private int _overchargedTime = 5;

    public float trackingTimer {get; set;}
    public float attackingTimer {get; set;}
    public float overchargedTimer {get; set;}



    private int _trackingSpeed; //not currently
    public bool _isChosen;




    /*

    public float Timer { get; set; }

private Update()
{
    if(Timer > 0.0f)
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
        {
            /// SWITCH STATE 
        }
    }
}




    */



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
        body.color = Color.white;

        if (_isChosen){

            trackingTimer = _trackingTime;
            currentState = FSMStates.Track;
        }
 
    }

    void UpdateTrackState(){

        print("Currently Tracking");

        FaceTarget(player.transform.position);
        body.color = Color.yellow;

        if(trackingTimer > 0.0f)
    {
        trackingTimer -= Time.deltaTime;
        if(trackingTimer <= 0)
        {
            attackingTimer = _attackingTime;
            currentState = FSMStates.Attack;
        }
    }
 
    }

    void UpdateAttackState(){

        print("Currently Attacking");
        body.color = Color.red;

           if(attackingTimer > 0.0f)
    {
        attackingTimer -= Time.deltaTime;
        if(attackingTimer <= 0)
        {
            overchargedTimer = _overchargedTime;
            currentState = FSMStates.Overcharged;
        }
    }
 
    }

    void UpdateOverchargedState(){

        print("Currently Overcharged");
        body.color = Color.blue;

           if(overchargedTimer > 0.0f)
    {
        overchargedTimer -= Time.deltaTime;
        if(overchargedTimer <= 0)
        {
            currentState = FSMStates.Neutral;
            _isChosen = false;
        }
    }
 
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
