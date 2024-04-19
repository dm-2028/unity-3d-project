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
        ContinueAnimation();
        stateMachine.inputReader.OnJumpPerformed += SwitchToJumpState;
        stateMachine.inputReader.OnAttackPerformed += Attack;
    }

    public override void Tick()
    {
        if (!OnGround && stateMachine.stepsSinceLastGrounded > 2)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
        CalculateMoveDirection();
        FaceMoveDirection();

        if (!stateMachine.isAttacking)
        {
            stateMachine.animator.SetFloat(moveSpeedHash, stateMachine.inputReader.movement.sqrMagnitude > 0f ? 1f : 0f, animationDampTime, Time.deltaTime);
            stateMachine.animator.speed = stateMachine.inputReader.movement.sqrMagnitude > 0f ? 2f : 1f;
        }
    }
    public override void TickFixed()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(stateMachine.transform.position, 1.0f);
        bool npcFound = false;
        foreach (Collider collider in nearbyObjects)
        {
            if (collider.transform.CompareTag("NPC"))
            {
                float dot = -Vector3.Dot(stateMachine.transform.forward, (collider.transform.position - stateMachine.transform.position).normalized);
                if (dot > 0.6f)
                {
                    stateMachine.SetNPCToTalkTo(collider.gameObject);
                    npcFound = true;
                }
            }
        }
        if (!npcFound)
        {
            stateMachine.RemoveNPCFromRange();
        }
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
        stateMachine.inputReader.OnAttackPerformed -= Attack;
    }

    public override void ContinueAnimation()
    {
        stateMachine.animator.speed = stateMachine.baseAnimationSpeed;
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);
    }
}
