using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int walkHash = Animator.StringToHash("Walk");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(walkHash, crossFadeDuration);
        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
        ignoreLayers = 1 << 10;
    }

    public override void Tick()
    {
        CalculateMoveDirection();
        //if (!stateMachine.controller.isGrounded)
        //{
        //    stateMachine.SwitchState(new PlayerFallState(stateMachine));
        //}
        //CalculateMoveDirection();
        //FaceMoveDirection();
        //Move();
        //CheckForClimb();

        //stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);

    }
    public override void TickFixed()
    {
        Debug.Log("tick fixed");
        upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(stateMachine.body.position, out upAxis);
        UpdateState();

        CalcVelocity();


        if (velocity.sqrMagnitude < 0.01f)
        {
            velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        }
        else
        {
            velocity += gravity * Time.deltaTime;
        }
        Debug.Log("velocity: " + velocity.ToString());

        stateMachine.body.velocity = velocity;
        ClearState();
    }

    void UpdateState()
    {
        stateMachine.stepsSinceLastGrounded += 1;
        stateMachine.stepsSinceLastJump += 1;
        velocity = stateMachine.body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stateMachine.stepsSinceLastGrounded = 0;
            if (stateMachine.stepsSinceLastJump > 1)
            {
                stateMachine.jumpPhase = 0;
            }
            if (stateMachine.groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }

        if (connectedBody)
        {
            if (connectedBody.isKinematic || connectedBody.mass >= stateMachine.body.mass)
            {
                UpdateConnectionState();
            }
        }
    }

    public override void Exit()
    {
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
    }

    private void SwitchToJumpState()
    {
        Debug.Log("switch to jump state");
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }

    protected void CalcVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;
        Debug.Log("player input " + playerInput.ToString());

        acceleration = OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration;
        speed = OnGround && desiresClimbing ? stateMachine.maxClimbSpeed : stateMachine.maxSpeed;
        xAxis = rightAxis;
        zAxis = forwardAxis;
        
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;
    }
}
