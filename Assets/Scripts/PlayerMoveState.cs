using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.velocity.y = Physics.gravity.y;
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);
        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
    }

    public override void Tick()
    {
        if (!stateMachine.controller.isGrounded)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        CalculateMoveDirection();
        FaceMoveDirection();
        Move();

        stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);

    }

    public override void Exit()
    {
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
    }

    private void SwitchToJumpState()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }
}
