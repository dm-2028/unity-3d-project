using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RatStateMachine : EnemyStateMachine, IHitboxResponder
{
    private readonly int hitHash = Animator.StringToHash("Hit");

    public NavMeshAgent agent { get; private set; }
    public EnemySpawn enemySpawn { get; set; }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        hitbox = GetComponentInChildren<HitBox>();
        hitbox.UseResponder(this);

        SwitchState(new RatWanderState(this));
    }

    public override void ReceiveDamage(int damage)
    {
        if (IsDead) return;
        Debug.Log("Receiving damage " + health);
        health -= damage;
        Debug.Log("health " + health);
        takingDamage = true;
        if (health <= 0)
        {
            IsDead = true;
            enemySpawn.incrementKilled(gameObject);
            SwitchState(new RatDeadState(this));
        }
        else
        {
            animator.speed = baseSpeed * 1.5f;
            agent.speed = baseSpeed * 1.5f;
            agent.isStopped = true;
            animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
        }
    }

    public override void ResetHurt()
    {
        takingDamage = false;
        ((RatBaseState)currentState)?.ContinueAnimation();
    }


}
