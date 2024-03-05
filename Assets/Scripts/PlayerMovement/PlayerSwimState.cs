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


    public PlayerSwimState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("enter swim state");

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
        FaceMoveDirection();


        stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);
        stateMachine.animator.speed = stateMachine.inputReader.movement.sqrMagnitude > 0f ? .5f : .25f;
        Debug.Log("velocity in tick " + stateMachine.velocity + "\nand " + stateMachine.body.velocity);
    }

    public override void TickFixed()
    {
        stateMachine.upAxis = -Physics.gravity.normalized;
        UpdateState();
        stateMachine.velocity *= 1f - stateMachine.waterDrag * stateMachine.submergence * Time.deltaTime; 
        float swimFactor = Mathf.Min(1f, stateMachine.submergence / stateMachine.swimThreshold);
        float speed = Mathf.LerpUnclamped(stateMachine.maxSpeed, stateMachine.maxSwimSpeed, swimFactor);
        CalcVelocity(Mathf.LerpUnclamped(
            OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration, stateMachine.maxSwimAcceleration, swimFactor), speed);
        stateMachine.body.velocity = stateMachine.velocity;


        Vector3 relativeVelocity = stateMachine.velocity - stateMachine.connectionVelocity;
        float yMovement = playerInput.y * speed - Vector3.Dot(relativeVelocity, stateMachine.upAxis);

        if (Swimming)
        {
            Debug.Log("body position " + stateMachine.body.position.y + "\nwater surface " + waterSurface);
            if(stateMachine.body.position.y >= waterSurface)
            {
                yMovement = Mathf.Min(yMovement, 0f);
            }
            stateMachine.velocity += stateMachine.upAxis * yMovement;
        }
        Debug.Log("velocity in tick fixed " + stateMachine.velocity + "\nand " + stateMachine.body.velocity);

        ClearState();
    }
    void EvaluateJump()
    {
        if (stateMachine.body.position.y >= waterSurface)
        {
            stateMachine.SwitchState(new PlayerJumpState(stateMachine));
        }
    }

    public override void Exit()
    {
        Debug.Log("exiting swim state");
        stateMachine.inputReader.OnJumpPerformed -= EvaluateJump;
    }

}
