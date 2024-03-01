using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int fallHash = Animator.StringToHash("Fall");
    private const float crossFadeDuration = 0.1f;

    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        velocity.y = 0f;
        stateMachine.animator.CrossFadeInFixedTime(fallHash, crossFadeDuration);
        //stateMachine.inputReader.OnJumpPerformed += SwitchToDoubleJumpState;
    }
    public override void Tick()
    {
        //ApplyGravity();
        //CalculateMoveDirection();
        //FaceMoveDirection();
        //Move();

        //if (stateMachine.controller.isGrounded)
        //{
        //    stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        //}
    }

    public override void Exit()
    {
        //stateMachine.inputReader.OnJumpPerformed -= SwitchToDoubleJumpState;
    }

    //private void SwitchToDoubleJumpState()
    //{
    //    stateMachine.SwitchState(new PlayerDoubleJumpState(stateMachine));
    //}

    public override void TickFixed()
    {
        throw new System.NotImplementedException();
    }
}
