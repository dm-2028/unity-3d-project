using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private readonly int jumpHash = Animator.StringToHash("Jump");
    private const float crossFadeDuration = 0.1f;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("enter jump state");
        stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.jumpForce, stateMachine.velocity.z);

        stateMachine.animator.CrossFadeInFixedTime(jumpHash, crossFadeDuration);

        stateMachine.inputReader.OnJumpPerformed += SwitchToDoubleJumpState;
    }

    public override void Tick()
    {
        ApplyGravity();

        if(stateMachine.velocity.y <= 0f)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        CalculateMoveDirection();
        FaceMoveDirection();
        Move();
    }

    public override void Exit()
    {
        stateMachine.inputReader.OnJumpPerformed -= SwitchToDoubleJumpState;
    }
    private void SwitchToDoubleJumpState()
    {
        stateMachine.SwitchState(new PlayerDoubleJumpState(stateMachine));
    }
}
