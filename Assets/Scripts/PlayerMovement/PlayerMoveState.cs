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
        stateMachine.animator.speed = stateMachine.baseAnimationSpeed;
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);

        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
        stateMachine.inputReader.OnAttackPerformed += SwitchToAttackState;
    }

    public override void Tick()
    {
        //Debug.Log("steps since last grounded: " + stateMachine.stepsSinceLastGrounded + "\n" + "ground contact count " + stateMachine.groundContactCount + "\ncontact normal: " + stateMachine.contactNormal);
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

        CalcVelocity(stateMachine.maxAcceleration, stateMachine.maxSpeed, stateMachine.rightAxis, stateMachine.forwardAxis);


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

    public override void Exit()
    {
        stateMachine.animator.speed = 1f;
        Debug.Log("exit move state");
        stateMachine.inputReader.OnJumpPerformed -= SwitchToJumpState;
        stateMachine.inputReader.OnAttackPerformed -= SwitchToAttackState;
    }
}
