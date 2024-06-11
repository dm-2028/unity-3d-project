using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSurfaceSwimState : PlayerBaseState
{

    private readonly int swimBlendTreeHash = Animator.StringToHash("SwimBlendTree");
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;


    int stepsSinceLastSwimming;


    public PlayerSurfaceSwimState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("enter swim state");
        stateMachine.splashParticles.Play();
        stateMachine.waveParticles.Play();
        stateMachine.waveParticles.gameObject.transform.position = new(stateMachine.waveParticles.transform.position.x, stateMachine.bodyOfWaterSurface.Value + .01f, stateMachine.waveParticles.transform.position.z);
        stateMachine.velocity = new Vector3(stateMachine.velocity.x, 0f, stateMachine.velocity.z);

        stateMachine.velocity = new Vector3(stateMachine.velocity.x, 0f, stateMachine.velocity.z);
        Debug.Log("state machine wave" + stateMachine.waveParticles.gameObject.transform.position);
        stateMachine.body.velocity = stateMachine.velocity;

        stateMachine.inputReader.OnJumpPerformed += Jump;
        stateMachine.animator.CrossFadeInFixedTime(swimBlendTreeHash, crossFadeDuration);
    }

    public override void Tick()
    {
        CalculateMoveDirection();
        FaceMoveDirection();
        SetPosition();
        Debug.Log("body position " + stateMachine.body.position.y);

        stateMachine.transform.position = new(stateMachine.transform.position.x, Mathf.Lerp(stateMachine.transform.position.y, stateMachine.bodyOfWaterSurface.Value, Time.deltaTime*100), stateMachine.transform.position.z);
        stateMachine.waveParticles.transform.position = new(stateMachine.waveParticles.transform.position.x, Mathf.Lerp(stateMachine.waveParticles.transform.position.y, stateMachine.bodyOfWaterSurface.Value + .01f, Time.deltaTime*100), stateMachine.waveParticles.transform.position.z);
        Debug.Log("state machine wave" + stateMachine.waveParticles.transform.position);
        Debug.Log("enter state body position " + stateMachine.transform.position.y);

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
    void Jump()
    {
        jumping = true;
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }

    public override void Exit()
    {
        Debug.Log("exiting swim state");
        stateMachine.jumpFromSwim = true;
        stateMachine.contactNormal = stateMachine.upAxis;
        stateMachine.waveParticles.Stop();
        stateMachine.inputReader.OnJumpPerformed -= Jump;
    }

}
