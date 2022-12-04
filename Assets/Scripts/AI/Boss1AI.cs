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
        Overcharged,
        Hit
    }

    [SerializeField] GameObject player;
    [SerializeField] Boss1Health bossHealth;
    public FSMStates currentState;

    public SpriteRenderer body;
    public GameObject lazer;

    private int _trackingTime = 5;
    private int _attackingTime = 5;
    private int _lazerTime = 1;
    private int _overchargedTime = 5;
    private int _hitTime = 2;

    public float trackingTimer {get; set;}
    public float attackingTimer {get; set;}
    public float lazerTimer {get; set;}
    public float overchargedTimer {get; set;}
    public float hitTimer {get; set;}



    private int _trackingSpeed; //not currently used
    public bool _isChosen;
    public bool _hitWithValve;


    // Start is called before the first frame update
    void Start()
    {

        currentState = FSMStates.Neutral;
        _hitWithValve = false;
        _isChosen = false;
        
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
            case FSMStates.Hit:
                UpdateHitState();
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
            lazerTimer = _lazerTime;
            currentState = FSMStates.Attack;
        }
    }
 
    }

    void UpdateAttackState(){

        print("Currently Attacking");
        body.color = Color.red;

        if(lazerTimer > 0.0f)
    {
        lazerTimer -= Time.deltaTime;
        if(lazerTimer <= 0)
        {
            lazer.SetActive(true);
            
        }
    }
        

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
        lazer.SetActive(false);

        if(_hitWithValve){

            overchargedTimer = 0;
            currentState = FSMStates.Hit;
            hitTimer = _hitTime;
            bossHealth.TakeDamage();
        }

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

    void UpdateHitState(){

        print("Currently Hit");
        body.color = Color.black;
        


         if(hitTimer > 0.0f)
    {
        hitTimer -= Time.deltaTime;
        if(hitTimer <= 0)
        {
            currentState = FSMStates.Neutral;
            _isChosen = false;
            _hitWithValve = false;
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
