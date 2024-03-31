using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyBaseState
{
    private readonly int hitHash = Animator.StringToHash("Hit");

    private const float crossFadeDuration = 0.1f;

    public EnemyHitState(EnemyStateMachine stateMachine) : base(stateMachine) { }


    public override void Enter()
    {
        stateMachine.animator.speed = stateMachine.baseSpeed;
        stateMachine.agent.speed = stateMachine.baseSpeed;
        stateMachine.agent.isStopped = true;
        stateMachine.animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        if (!AnimatorIsPlaying())
        {
            stateMachine.SwitchState(new EnemyWanderState(stateMachine));
        }
    }

    public override void TickFixed()
    {
    }

    bool AnimatorIsPlaying()
    {
        return stateMachine.animator.GetCurrentAnimatorStateInfo(0).length >
               stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
