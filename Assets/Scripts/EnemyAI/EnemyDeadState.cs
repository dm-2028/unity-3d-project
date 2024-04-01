using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    private readonly int deadHash = Animator.StringToHash("Dead");

    private const float crossFadeDuration = 0.1f;

    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.speed = stateMachine.baseSpeed;
        stateMachine.agent.speed = stateMachine.baseSpeed;
        stateMachine.agent.isStopped = true;
        stateMachine.animator.CrossFadeInFixedTime(deadHash, crossFadeDuration);
        stateMachine.Invoke("DestroyEnemy", 5);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }

    public override void TickFixed()
    {
    }


}
