using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : StateMachine
{
    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public EnemySpawn enemySpawn { get; set; }


    int health = 2;

}
