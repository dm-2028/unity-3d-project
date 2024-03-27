using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("Movement");
    private readonly int moveBlendTreeHash = Animator.StringToHash("EnemhyMoveBlendTree");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public EnemyMoveState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, moveSpeedHash);
    }

    public override void Tick()
    {
    }
    public override void TickFixed()
    {
    }

    public override void Exit()
    {
    }
}
