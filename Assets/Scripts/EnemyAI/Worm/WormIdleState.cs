using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormIdleState : WormBaseState
{
    private readonly int idleHash = Animator.StringToHash("WormIdle");
    private readonly int attackHash = Animator.StringToHash("WormAttack");
    private readonly int slamHash = Animator.StringToHash("WormSlam");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public WormIdleState(WormStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        ContinueAnimation();
    }

    void Attack()
    {
        stateMachine.isAttacking = true;
        stateMachine.inCooldown = true;
        stateMachine.Invoke("ResetCooldown", stateMachine.cooldownTime + Random.Range(-1.0f, 1.0f));
        stateMachine.animator.CrossFadeInFixedTime(attackHash, crossFadeDuration);

    }
    void SlamAttack()
    {
        Debug.Log("slam attack");
        stateMachine.isAttacking = true;
        stateMachine.inCooldown = true;
        stateMachine.Invoke("ResetCooldown", stateMachine.cooldownTime + Random.Range(-1.0f, 1.0f));
        stateMachine.animator.CrossFadeInFixedTime(slamHash, crossFadeDuration);
    }
    public override void Exit()
    {
    }

    public override void Tick()
    {
        RotateTowardsPlayer();
        if (!stateMachine.inCooldown)
        {
            float distance = (stateMachine.transform.position - stateMachine.player.transform.position).magnitude;
            if (distance < 2)
            {
                Attack();
            }else if(distance < 10)
            {
                SlamAttack();
            }
        }

    }

    public override void TickFixed()
    {
    }

    public override void ContinueAnimation()
    {
        stateMachine.animator.CrossFadeInFixedTime(idleHash, crossFadeDuration);
        stateMachine.animator.speed = stateMachine.baseSpeed;

    }
}
