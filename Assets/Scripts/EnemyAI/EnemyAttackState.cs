using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    private readonly int attackLHash = Animator.StringToHash("Attack L");
    private readonly int attackRHash = Animator.StringToHash("Attack R");

    private readonly int moveBlendTreeHash = Animator.StringToHash("EnemhyMoveBlendTree");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    private bool attackL;
    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine) { }

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
        stateMachine.inCooldown = true;
        stateMachine.Invoke("resetCooldown", stateMachine.cooldownTime);
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
                stateMachine.SwitchState(new EnemyWanderState(stateMachine));
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
