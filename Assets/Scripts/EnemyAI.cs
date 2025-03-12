using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyAI;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent ai;
    public Transform patrolPoint;
    public enum EnemyState
    {
        Idle,
        Walk,
        Chasing,
        Attack
    };
    public EnemyState enemyState;
    private Animator anim;
    private float distanceToTarget;
    private Coroutine idleToPatrol;

    void Start()
    {
        ai = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyState = EnemyState.Idle;
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
    }

    private void SwitchState(int newState)
    {
        if (anim.GetInteger("State") != newState)
        {
            anim.SetInteger("State", newState);
        }
    }

    void Update()
    {
        if (target != null)
        {
            ai.SetDestination(target.position);
        }
        distanceToTarget = Mathf.Abs(Vector3.Distance(target.position, transform.position));
        //reviwe below//
        switch (enemyState)
        {
            case EnemyState.Idle:
                SwitchState(0);
                ai.SetDestination(transform.position);
                if (idleToPatrol == null)
                {
                    idleToPatrol = StartCoroutine(SwitchToPatrol());
                }
                break;

            case EnemyState.Walk:
                float distanceToPatrolPoint = Mathf.Abs(Vector3.Distance(patrolPoint.position, transform.position));
                if (distanceToPatrolPoint > 2)
                {
                    SwitchState(1);
                    ai.SetDestination(patrolPoint.position);
                }
                else
                {
                    SwitchState(0);
                }

                if (distanceToTarget <= 15)
                {
                    enemyState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                SwitchState(2);

                ai.SetDestination(target.position);

                if (distanceToTarget <= 5)
                {
                    enemyState = EnemyState.Attack;
                }
                else if (distanceToTarget > 15)
                {
                    enemyState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attack:
                SwitchState(3);

                if (distanceToTarget > 5 && distanceToTarget <= 15)
                {
                    enemyState = EnemyState.Chasing;
                }
                else if (distanceToTarget > 15)
                {
                    enemyState = EnemyState.Idle;
                }
                break;

            default:
                Debug.LogWarning("Unknown enemy state");
                break;
        }
        //review above//
    }

    private IEnumerator SwitchToPatrol()
    {
        yield return new WaitForSeconds(5);
        enemyState = EnemyState.Walk;
        idleToPatrol = null;
    }
}
