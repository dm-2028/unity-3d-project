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
        Debug.Log("entering fall state");
        stateMachine.inputReader.OnJumpPerformed += CheckDoubleJump;

        stateMachine.animator.CrossFadeInFixedTime(fallHash, crossFadeDuration);

        if (stateMachine.maxAirJumps > 0 && stateMachine.jumpPhase <= stateMachine.maxAirJumps)
        {
            if (stateMachine.jumpPhase == 0)
            {
                stateMachine.jumpPhase = 1;
            }
        }
    }
    public override void Tick()
    {
        CalculateMoveDirection();
        FaceMoveDirection();

        if (OnGround)
        {
            stateMachine.audioSource.PlayOneShot(stateMachine.landSound);
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("exiting fall state");
        stateMachine.inputReader.OnJumpPerformed -= CheckDoubleJump;
    }

    public override void TickFixed()
    {
        stateMachine.upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(stateMachine.body.position, out stateMachine.upAxis);
        UpdateState();
        //CalcVelocity(stateMachine.maxAirAcceleration, stateMachine.maxSpeed, stateMachine.rightAxis, stateMachine.forwardAxis);
        stateMachine.velocity += gravity * Time.deltaTime;
        stateMachine.body.velocity = stateMachine.velocity;
        ClearState();
    }
}
