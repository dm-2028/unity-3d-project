using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttackState : RatBaseState
{
    private readonly int attackLHash = Animator.StringToHash("Attack L");
    private readonly int attackRHash = Animator.StringToHash("Attack R");

    private const float crossFadeDuration = 0.1f;

    private bool attackL;
    public RatAttackState(RatStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("enter attack state");
        stateMachine.animator.speed = stateMachine.baseSpeed;
        stateMachine.agent.speed = stateMachine.baseSpeed;
        stateMachine.agent.isStopped = true;
        attackL = Random.Range(0, 2) == 1;
        Attack();
    }

    void Attack()
    {
        stateMachine.isAttacking = true;
        stateMachine.inCooldown = true;
        stateMachine.Invoke("ResetCooldown", stateMachine.cooldownTime);
        stateMachine.animator.CrossFadeInFixedTime(attackL ? attackLHash : attackRHash, crossFadeDuration);
        attackL = !attackL;
    }

    public override void Tick()
    {
        if (!stateMachine.inCooldown)
        {
            if((stateMachine.transform.position - stateMachine.player.transform.position).magnitude < 1)
            {
                Attack();
            }
            else
            {
                stateMachine.SwitchState(new RatPursueState(stateMachine));
            }
        }
    }
    public override void TickFixed()
    {

    }

    public override void Exit()
    {

    }
}
