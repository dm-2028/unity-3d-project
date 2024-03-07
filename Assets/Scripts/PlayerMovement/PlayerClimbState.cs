using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveYHash = Animator.StringToHash("MoveY");
    private readonly int climbBlendTreeHash = Animator.StringToHash("ClimbBlendTree");
    private const float crossFadeDuration = 0.1f;
    private const float animationDampTime = 0.1f;

    public PlayerClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(climbBlendTreeHash, crossFadeDuration);
        Debug.Log("climbing " + Climbing + "\nsteepcontactcount " + stateMachine.climbContactCount);
        Debug.Log("enter climb state");
        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
    }

    public override void Tick()
    {

        CalculateMoveDirection();
        stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(stateMachine.climbNormal), stateMachine.lookRotationDampFactor * Time.deltaTime);

        stateMachine.animator.SetFloat(moveYHash, stateMachine.inputReader.movement.y > 0f ? 1f : stateMachine.inputReader.movement.y < 0f ? -1f : 0f, animationDampTime, Time.deltaTime);
        stateMachine.animator.SetFloat(moveXHash, stateMachine.inputReader.movement.x > 0f ? 1f : stateMachine.inputReader.movement.x < 0f ? -1f : 0f, animationDampTime, Time.deltaTime);
    }

    public override void TickFixed()
    {
        //Debug.Log("tick fixed " + stateMachine.body.velocity);
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

        CalcVelocity(stateMachine.maxClimbAcceleration, stateMachine.maxClimbSpeed, Vector3.Cross(stateMachine.contactNormal, stateMachine.upAxis), stateMachine.upAxis);

        stateMachine.velocity -= stateMachine.contactNormal * (stateMachine.maxClimbAcceleration * 0.45f * Time.deltaTime);

        stateMachine.body.velocity = stateMachine.velocity;

        ClearState();
    }

    public override void Exit()
    {
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
    }
}

