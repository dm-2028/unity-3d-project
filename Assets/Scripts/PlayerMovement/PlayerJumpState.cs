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


        stateMachine.inputReader.OnJumpPerformed += CheckDoubleJump;

        Vector3 jumpDirection;
        if (OnGround)
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
            Debug.Log("return without jumping");
            return;
        }
        Debug.Log("jump direction before " + jumpDirection);
        stateMachine.stepsSinceLastJump = 0;
        stateMachine.jumpPhase += 1;

        float jumpSpeed = Mathf.Sqrt(2f * Physics.gravity.magnitude * stateMachine.jumpHeight);
        Debug.Log("jump speed " + jumpSpeed);
        if (InWater)
        {
            jumpSpeed *= Mathf.Max(0f, 1f - stateMachine.submergence / stateMachine.swimThreshold);
        }
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
            Debug.Log("jump direction " + jumpDirection);
        stateMachine.velocity += jumpDirection * jumpSpeed;
        Debug.Log("velocity of jump " + stateMachine.velocity);
        stateMachine.body.velocity = stateMachine.velocity;
        stateMachine.animator.CrossFadeInFixedTime(jumpHash, crossFadeDuration);

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
        Debug.Log("velocity before update " + stateMachine.velocity);
        UpdateState();
        Debug.Log("velocity before adjust " + stateMachine.velocity);
        CalcVelocity();
        stateMachine.velocity += gravity * Time.deltaTime;
        Debug.Log("velocity in jump = " + stateMachine.velocity);
        stateMachine.body.velocity = stateMachine.velocity;
        ClearState();
    }

    void UpdateState()
    {
        stateMachine.stepsSinceLastGrounded += 1;
        stateMachine.stepsSinceLastJump += 1;
        stateMachine.velocity = stateMachine.body.velocity;
        if (CheckClimbing() || CheckSwimming() ||
            OnGround || CheckSteepContacts())
        {
            stateMachine.stepsSinceLastGrounded = 0;
            if (stateMachine.stepsSinceLastJump > 1)
            {
                stateMachine.jumpPhase = 0;
            }
            if (stateMachine.groundContactCount > 1)
            {
                stateMachine.contactNormal.Normalize();
            }
        }
        else
        {
            stateMachine.contactNormal = stateMachine.upAxis;
        }

        if (stateMachine.connectedBody)
        {
            if (stateMachine.connectedBody.isKinematic || stateMachine.connectedBody.mass >= stateMachine.body.mass)
            {
                UpdateConnectionState();
            }
        }
    }

    protected void CalcVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;

        if (Climbing)
        {
            acceleration = stateMachine.maxClimbAcceleration;
            speed = stateMachine.maxClimbSpeed;
            xAxis = Vector3.Cross(stateMachine.contactNormal, stateMachine.upAxis);
            zAxis = stateMachine.upAxis;
        }
        else if (InWater)
        {
            float swimFactor = Mathf.Min(1f, stateMachine.submergence / stateMachine.swimThreshold);
            acceleration = Mathf.LerpUnclamped(
                OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration, stateMachine.maxSwimAcceleration, swimFactor);
            speed = Mathf.LerpUnclamped(stateMachine.maxSpeed, stateMachine.maxSwimSpeed, swimFactor);
            xAxis = stateMachine.rightAxis;
            zAxis = stateMachine.forwardAxis;
        }
        else
        {
            acceleration = OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration;
            speed = OnGround && desiresClimbing ? stateMachine.maxClimbSpeed : stateMachine.maxSpeed;
            xAxis = stateMachine.rightAxis;
            zAxis = stateMachine.forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, stateMachine.contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, stateMachine.contactNormal);

        Vector3 relativeVelocity = stateMachine.velocity - stateMachine.connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, stateMachine.upAxis) : 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        stateMachine.velocity += xAxis * adjustment.x + zAxis * adjustment.z;

        if (Swimming)
        {
            stateMachine.velocity += stateMachine.upAxis * adjustment.y;
        }
    }
}
