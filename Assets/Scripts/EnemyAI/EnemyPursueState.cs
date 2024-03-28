using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("Movement");
    private readonly int moveHash = Animator.StringToHash("Move");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;


    public EnemyPursueState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("enter pursue state");
        stateMachine.animator.CrossFadeInFixedTime(moveHash, crossFadeDuration);
        stateMachine.animator.speed = stateMachine.baseSpeed * 1.5f;
        stateMachine.agent.speed = stateMachine.baseSpeed * 1.5f;
        stateMachine.agent.isStopped = false;
    }

    public override void Tick()
    {
        if ((stateMachine.transform.position - stateMachine.player.transform.position).magnitude > 1) 
        {
            stateMachine.agent.isStopped = false;
            stateMachine.agent.SetDestination(stateMachine.player.transform.position);
            Debug.Log(stateMachine.agent.destination + " is the destination");
        }
        else
        {
            stateMachine.SwitchState(new EnemyAttackState(stateMachine));
        }
        if (!CanSeePlayer())
        {
            stateMachine.SwitchState(new EnemyWanderState(stateMachine));
        }
    }
    public override void TickFixed()
    {

    }

    public override void Exit()
    {
    }
}
