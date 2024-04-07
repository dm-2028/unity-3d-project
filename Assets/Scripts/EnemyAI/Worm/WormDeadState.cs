using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormDeadState : WormBaseState
{
    private readonly int deadHash = Animator.StringToHash("WormDead");

    private const float crossFadeDuration = 0.1f;

    public WormDeadState(WormStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.speed = stateMachine.baseSpeed;
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

