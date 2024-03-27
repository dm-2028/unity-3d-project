using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : StateMachine
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        SwitchState(new EnemyMoveState(this));
    }
}
