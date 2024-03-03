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
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);

        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
        ignoreLayers = 1 << 10;
    }

    public override void Tick()
    {
        Debug.Log("steps since last grounded: " + stateMachine.stepsSinceLastGrounded + "\n" + "ground contact count " + stateMachine.groundContactCount + "\ncontact normal: " + stateMachine.contactNormal);
        if (!OnGround && stateMachine.stepsSinceLastGrounded > 2)
        {
            Debug.Log("switch to fall state move");
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        CalculateMoveDirection();
        FaceMoveDirection();

        stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);
        stateMachine.animator.speed = stateMachine.inputReader.movement.sqrMagnitude > 0f ? 2f : 1f;

    }
    public override void TickFixed()
    {
        stateMachine.upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(stateMachine.body.position, out stateMachine.upAxis);
        UpdateState();

        CalcVelocity();


        if (stateMachine.velocity.sqrMagnitude < 0.01f)
        {
            stateMachine.velocity += stateMachine.contactNormal * (Vector3.Dot(gravity, stateMachine.contactNormal) * Time.deltaTime);
        }
        else
        {
            stateMachine.velocity += gravity * Time.deltaTime;
        }

        stateMachine.body.velocity = stateMachine.velocity;
        ClearState();
    }

    void UpdateState()
    {
        stateMachine.stepsSinceLastGrounded += 1;
        stateMachine.stepsSinceLastJump += 1;
        stateMachine.velocity = stateMachine.body.velocity;
        if (!jumping && ( OnGround || SnapToGround() || CheckSteepContacts()))
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

    public override void Exit()
    {
        Debug.Log("exit move state");
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
    }

    protected void CalcVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;

        acceleration = stateMachine.maxAcceleration;
        speed = stateMachine.maxSpeed;
        xAxis = stateMachine.rightAxis;
        zAxis = stateMachine.forwardAxis;
        
        xAxis = ProjectDirectionOnPlane(xAxis, stateMachine.contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, stateMachine.contactNormal);

        Vector3 relativeVelocity = stateMachine.velocity - stateMachine.connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        stateMachine.velocity += xAxis * adjustment.x + zAxis * adjustment.z;
    }
}
