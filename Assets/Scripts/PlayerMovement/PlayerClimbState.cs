using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    private readonly int climbHash = Animator.StringToHash("ClimbBlendTree");
    private const float crossFadeDuration = 0.1f;

    public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        Debug.Log("climbing " + Climbing + "\nsteepcontactcount " + stateMachine.climbContactCount);
        Debug.Log("enter climb state");
        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
    }

    public override void Tick()
    {

        CalculateMoveDirection();
        FaceMoveDirection();
    }

    public override void TickFixed()
    {
        Debug.Log("tick fixed " + stateMachine.body.velocity);
        stateMachine.stepsSinceLastGrounded += 1;
        stateMachine.stepsSinceLastJump += 1;
        stateMachine.velocity = stateMachine.body.velocity;
        if (stateMachine.climbContactCount > 1)
        {
            stateMachine.climbNormal.Normalize();
            float upDot = Vector3.Dot(stateMachine.upAxis, stateMachine.climbNormal);
            if (upDot >= stateMachine.minGroundDotProduct)
            {
                stateMachine.climbNormal = stateMachine.lastClimbNormal;
            }
        }
        stateMachine.groundContactCount = stateMachine.climbContactCount;
        stateMachine.contactNormal = stateMachine.climbNormal;

        if (stateMachine.connectedBody)
        {
            if (stateMachine.connectedBody.isKinematic || stateMachine.connectedBody.mass >= stateMachine.body.mass)
            {
                UpdateConnectionState();
            }
        }
        Debug.Log("state machine " + stateMachine.velocity);
        CalcVelocity(stateMachine.maxClimbAcceleration, stateMachine.maxClimbSpeed, Vector3.Cross(stateMachine.contactNormal, stateMachine.upAxis), stateMachine.upAxis);
        Debug.Log("state machine after " + stateMachine.velocity);


        stateMachine.velocity -= stateMachine.contactNormal * (stateMachine.maxClimbAcceleration * 0.9f * Time.deltaTime);

        stateMachine.body.velocity = stateMachine.velocity;

        Debug.Log("body velocity " + stateMachine.body.velocity);
        ClearState();
    }

    public override void Exit()
    {
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
    }
    //void AdjustVelocity()
    //{
    //    float acceleration, speed;
    //    Vector3 xAxis, zAxis;

    //    if (Climbing)
    //    {
    //        acceleration = maxClimbAcceleration;
    //        speed = maxClimbSpeed;
    //        xAxis = Vector3.Cross(contactNormal, upAxis);
    //        zAxis = upAxis;
    //    }
    //    else if (InWater)
    //    {
    //        float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
    //        acceleration = Mathf.LerpUnclamped(
    //            OnGround ? maxAcceleration : maxAirAcceleration, maxSwimAcceleration, swimFactor);
    //        speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
    //        xAxis = rightAxis;
    //        zAxis = forwardAxis;
    //    }
    //    else
    //    {
    //        acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
    //        speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed;
    //        xAxis = rightAxis;
    //        zAxis = forwardAxis;
    //    }
    //    xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
    //    zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

    //    Vector3 relativeVelocity = velocity - connectionVelocity;

    //    Vector3 adjustment;
    //    adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
    //    adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
    //    adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

    //    adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

    //    velocity += xAxis * adjustment.x + zAxis * adjustment.z;

    //    if (Swimming)
    //    {
    //        velocity += upAxis * adjustment.y;
    //    }
    //}
}

