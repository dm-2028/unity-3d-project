using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimState : PlayerBaseState
{

    private readonly int swimBlendTreeHash = Animator.StringToHash("SwimBlendTree");
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    float waterSurface;

    int stepsSinceLastSwimming;


    public PlayerSwimState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("enter swim state");
        stateMachine.particles.Play();
        waterSurface = stateMachine.body.position.y;
        Debug.Log("enter state body position " + stateMachine.body.position.y + "\nwater surface " + waterSurface);
        stateMachine.velocity = new Vector3(stateMachine.velocity.x, 0f, stateMachine.velocity.z);
        stateMachine.body.velocity = stateMachine.velocity;
        stateMachine.inputReader.OnJumpPerformed += EvaluateJump;
        stateMachine.animator.CrossFadeInFixedTime(swimBlendTreeHash, crossFadeDuration);
    }

    public override void Tick()
    {
        CalculateMoveDirection();
        playerInput.y = stateMachine.inputReader.verticalMovement;
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        FaceMoveDirection();
        SetPosition();

        stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);
        stateMachine.animator.speed = stateMachine.inputReader.movement.sqrMagnitude > 0f ? .5f : .25f;
        stateMachine.animator.speed = stateMachine.inputReader.movement.sqrMagnitude > 0f ? .5f : .25f;
        Debug.Log("velocity in tick " + stateMachine.velocity + "\nand " + stateMachine.body.velocity);
    }

    public override void TickFixed()
    {
        stateMachine.upAxis = -Physics.gravity.normalized;
        stateMachine.stepsSinceLastJump += 1;
        stateMachine.velocity = stateMachine.body.velocity;

        if (!CheckSwimming())
        {
            if (OnGround)
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));
            }
        }

        stateMachine.stepsSinceLastGrounded = 0;
        if (stateMachine.stepsSinceLastJump > 1)
        {
            stateMachine.jumpPhase = 0;
        }
        if (stateMachine.groundContactCount > 1)
        {
            stateMachine.contactNormal.Normalize();
        }

        if (stateMachine.connectedBody)
        {
            if (stateMachine.connectedBody.isKinematic || stateMachine.connectedBody.mass >= stateMachine.body.mass)
            {
                UpdateConnectionState();
            }
        }

        stateMachine.velocity *= 1f - stateMachine.waterDrag * stateMachine.submergence * Time.deltaTime; 
        float swimFactor = Mathf.Min(1f, stateMachine.submergence / stateMachine.swimThreshold);
        float speed = Mathf.LerpUnclamped(stateMachine.maxSpeed, stateMachine.maxSwimSpeed, swimFactor);
        CalcVelocity(Mathf.LerpUnclamped(
            OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration, stateMachine.maxSwimAcceleration, swimFactor), speed, stateMachine.rightAxis, stateMachine.forwardAxis);
        stateMachine.body.velocity = stateMachine.velocity;


        Vector3 relativeVelocity = stateMachine.velocity - stateMachine.connectionVelocity;
        float yMovement = playerInput.y * speed - Vector3.Dot(relativeVelocity, stateMachine.upAxis);
        Debug.Log("ymovement " + yMovement);
        if (!Swimming && yMovement >= 0)
        {

            stateMachine.body.velocity = new(stateMachine.body.velocity.x, 0f, stateMachine.body.velocity.z);
        }
        else
        {
            stateMachine.velocity += stateMachine.upAxis * yMovement;
        }
        Debug.Log("velocity in tick fixed " + stateMachine.velocity + "\nand " + stateMachine.body.velocity);

        ClearState();
    }
    void EvaluateJump()
    {
        if (!Swimming)
        {
            jumping = true;
            stateMachine.SwitchState(new PlayerJumpState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("exiting swim state");
        stateMachine.jumpFromSwim = true;
        stateMachine.contactNormal = stateMachine.upAxis;
        stateMachine.inputReader.OnJumpPerformed -= EvaluateJump;
    }

}
