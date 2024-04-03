using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyStateMachine : StateMachine
{
    private readonly int hitHash = Animator.StringToHash("Hit");

    private const float crossFadeDuration = 0.1f;

    public Animator animator { get; private set; }
    public NavMeshAgent agent { get; private set; }
    public EnemySpawn enemySpawn { get; set; }

    [Range(0f, 5.0f)]
    public float cooldownTime = 1f;

    public bool inCooldown { get; set; } = false;
    public bool isDead { get; set; } = false;
    public bool takingDamage { get; set; } = false;
    public GameObject player;
    public float seeDistance = 10f;
    public int health = 2;
    public float baseSpeed = 1f;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        SwitchState(new EnemyWanderState(this));
    }
    void ResetCooldown()
    {
        inCooldown = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void ReceiveDamage()
    {
        Debug.Log("Receiving damage");
        health--;
        takingDamage = true;
        if (health <= 0)
        {
            enemySpawn.incrementKilled(gameObject);
            SwitchState(new EnemyDeadState(this));
        }
        else
        {
            animator.speed = baseSpeed * 1.5f;
            agent.speed = baseSpeed * 1.5f;
            agent.isStopped = true;
            animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
        }
    }

    public void ResetHurt()
    {
        takingDamage = false;
        ((EnemyBaseState)currentState)?.ContinueAnimation();
    }
}
