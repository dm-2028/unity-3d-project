using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkState : PlayerBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public PlayerTalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        ContinueAnimation();
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

    public override void ContinueAnimation()
    {
        stateMachine.animator.speed = stateMachine.baseAnimationSpeed;
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);
    }
}
