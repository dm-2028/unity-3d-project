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

    [Range(0f, 5.0f)]
    public float cooldownTime = 1f;

    public bool inCooldown { get; set; } = false;

    public GameObject player;
    public float seeDistance = 10f;
    int health = 2;
    public float baseSpeed = 1f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        SwitchState(new EnemyWanderState(this));
    }
    void resetCooldown()
    {
        Debug.Log("resetting cooldown");
        inCooldown = false;
    }

    void receiveDamage()
    {
        SwitchState(new EnemyHitState(this));
    }
}
