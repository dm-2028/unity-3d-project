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
        Vector3 jumpDirection;
        Debug.Log("on ground " + OnGround + " OnSteep " + OnSteep);
        if (OnGround)
        {
            jumpDirection = contactNormal;
        }
        else if (OnSteep)
        {
            jumpDirection = steepNormal;
            stateMachine.jumpPhase = 0;
        }
        else if (stateMachine.maxAirJumps > 0 && stateMachine.jumpPhase <= stateMachine.maxAirJumps)
        {
            if (stateMachine.jumpPhase == 0)
            {
                stateMachine.jumpPhase = 1;
            }
            jumpDirection = contactNormal;
        }
        else
        {
            return;
        }

        stateMachine.stepsSinceLastJump = 0;
        stateMachine.jumpPhase += 1;

        float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * stateMachine.jumpHeight);
        if (InWater)
        {
            jumpSpeed *= Mathf.Max(0f, 1f - submergence / stateMachine.swimThreshold);
        }
        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        velocity += jumpDirection * jumpSpeed;
        Debug.Log("velocity of jump " + velocity);

        stateMachine.animator.CrossFadeInFixedTime(jumpHash, crossFadeDuration);

    }

    public override void Tick()
    {
        //ApplyGravity();

        //if(stateMachine.velocity.y <= 0f)
        //{
        //    stateMachine.SwitchState(new PlayerFallState(stateMachine));
        //}
        //CalculateMoveDirection();
        //FaceMoveDirection();
        //Move();
        //CheckForClimb();
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
        upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(stateMachine.body.position, out upAxis);
        //UpdateState();
        AdjustVelocity();
        velocity += gravity * Time.deltaTime;
        stateMachine.body.velocity = velocity;
        ClearState();
    }
}
