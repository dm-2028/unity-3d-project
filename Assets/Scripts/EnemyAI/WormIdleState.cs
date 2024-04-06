using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormIdleState : WormBaseState
{
    private readonly int idleHash = Animator.StringToHash("WormIdle");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public WormIdleState(WormStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("enter idle state");
        ContinueAnimation();
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        RotateTowardsPlayer();

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
