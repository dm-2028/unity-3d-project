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
        jumping = true;

        stateMachine.inputReader.OnJumpPerformed += CheckDoubleJump;

        Vector3 jumpDirection;
        if (OnGround || stateMachine.jumpFromSwim)
        {
            jumpDirection = stateMachine.contactNormal;
        }
        else if (OnSteep)
        {
            jumpDirection = stateMachine.steepNormal;
            stateMachine.jumpPhase = 0;
        }
        else if (stateMachine.maxAirJumps > 0 && stateMachine.jumpPhase <= stateMachine.maxAirJumps)
        {
            if (stateMachine.jumpPhase == 0)
            {
                stateMachine.jumpPhase = 1;
            }
            jumpDirection = stateMachine.contactNormal;
        }
        else
        {
            return;
        }
        stateMachine.stepsSinceLastJump = 0;
        stateMachine.jumpPhase += 1;

        float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * stateMachine.jumpHeight);

        jumpDirection = (jumpDirection + stateMachine.upAxis).normalized;
        float alignedSpeed = Vector3.Dot(stateMachine.velocity, jumpDirection);
        if(stateMachine.velocity.y < 0)
        {
            stateMachine.velocity += new Vector3(0f, -stateMachine.velocity.y, 0f);
        }
        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }
        stateMachine.audioSource.PlayOneShot(stateMachine.jumpSound);

        stateMachine.velocity += jumpDirection * jumpSpeed;
        stateMachine.body.velocity = stateMachine.velocity;
        stateMachine.animator.CrossFadeInFixedTime(jumpHash, crossFadeDuration);
        stateMachine.jumpFromSwim = false;
    }



    public override void Tick()
    {
        CalculateMoveDirection();
    }

    public override void Exit()
    {
        Debug.Log("exit jump state");
        stateMachine.inputReader.OnJumpPerformed -= CheckDoubleJump;
    }

    public override void TickFixed()
    {
        if (OnGround && stateMachine.stepsSinceLastGrounded > 1 && stateMachine.stepsSinceLastJump > 2 )
        {
            Debug.Log("switching states move");
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }else if(stateMachine.body.velocity.y < 0){
            Debug.Log("Switching states fall");
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        stateMachine.upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(stateMachine.body.position, out stateMachine.upAxis);
        //Debug.Log("velocity before update " + stateMachine.velocity);
        UpdateState();
        //Debug.Log("velocity before adjust " + stateMachine.velocity);
        CalcVelocity(stateMachine.maxAirAcceleration, stateMachine.maxSpeed, stateMachine.rightAxis, stateMachine.forwardAxis);
        stateMachine.velocity += gravity * Time.deltaTime;
        //Debug.Log("velocity in jump = " + stateMachine.velocity);
        stateMachine.body.velocity = stateMachine.velocity;
        ClearState();
    }
}
