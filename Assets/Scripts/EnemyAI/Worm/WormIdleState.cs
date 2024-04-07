using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormIdleState : WormBaseState
{
    private readonly int idleHash = Animator.StringToHash("WormIdle");
    private readonly int attackHash = Animator.StringToHash("WormAttack");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public WormIdleState(WormStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("enter idle state");
        ContinueAnimation();
    }

    void Attack()
    {
        stateMachine.isAttacking = true;
        stateMachine.inCooldown = true;
        stateMachine.Invoke("ResetCooldown", stateMachine.cooldownTime + Random.Range(-1.0f, 1.0f));
        stateMachine.animator.CrossFadeInFixedTime(attackHash, crossFadeDuration);
    }
    public override void Exit()
    {
    }

    public override void Tick()
    {
        RotateTowardsPlayer();
        if (!stateMachine.inCooldown)
        {
            if ((stateMachine.transform.position - stateMachine.player.transform.position).magnitude < 2)
            {
                Attack();
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
