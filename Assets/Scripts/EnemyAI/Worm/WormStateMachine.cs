using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormStateMachine : EnemyStateMachine, IHitboxResponder
{
    private readonly int appearHash = Animator.StringToHash("Appear");
    private readonly int hitHash = Animator.StringToHash("WormHurt");

    public EnemySpawn enemySpawn { get; set; }

    public GameObject worm;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        hitbox = GetComponentInChildren<HitBox>();
        hitbox.UseResponder(this);
        animator.Play(appearHash);
    }

    public override void ReceiveDamage()
    {
        if (IsDead) return;
        Debug.Log("Receiving damage " + health);
        health--;
        Debug.Log("health " + health);
        takingDamage = true;
        if (health <= 0)
        {
            IsDead = true;
            //enemySpawn.incrementKilled(gameObject);
            SwitchState(new WormDeadState(this));
        }
        else
        {
            animator.speed = baseSpeed * 1.5f;
            animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
        }
    }

    public override void ResetHurt()
    {
        takingDamage = false;
        ((WormBaseState)currentState)?.ContinueAnimation();
    }


    public override void ContinueAnimation()
    {
        ((WormBaseState)currentState)?.ContinueAnimation();
    }
}
