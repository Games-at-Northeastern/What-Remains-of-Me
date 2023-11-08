using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateEnemyOnDoorOpen : MonoBehaviour
{
    [SerializeField] private Collider2D _doorCollider;

    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject _babyRobot;
    [SerializeField] private GameObject _infectedRobotWithAI;

    private void Update()
    {
        Debug.Log("door is: " + _doorCollider.enabled);
        if (_doorCollider.enabled == false)
        {
            _animator.Play("EnemyWakeUpAnimation");

        }

    }

    public void ActivateRobot()
    {

        _infectedRobotWithAI.SetActive(true);

        _babyRobot.SetActive(false);
    }
}
