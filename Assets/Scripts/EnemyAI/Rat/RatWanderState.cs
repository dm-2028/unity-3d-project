using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatWanderState : RatBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("Movement");
    private readonly int moveHash = Animator.StringToHash("Move");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public float wanderRadius = 5;
    public float wanderDistance = 10;
    public float wanderJitter = 1;
    Vector3 wanderTarget = Vector3.zero;

    public RatWanderState(RatStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        ContinueAnimation();
    }

    public override void Tick()
    {
        if (!stateMachine.takingDamage)
        {
            wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0, Random.Range(-1f, 1f) * wanderJitter);

            wanderTarget.Normalize();
            wanderTarget *= wanderRadius;

            Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
            Vector3 targetWorld = stateMachine.gameObject.transform.TransformPoint(targetLocal);
            stateMachine.agent.SetDestination(targetWorld);
            stateMachine.agent.isStopped = false;
            if (CanSeePlayer())
            {
                stateMachine.SwitchState(new RatPursueState(stateMachine));
            }
        }
    }
    public override void TickFixed(){ }

    public override void Exit(){ }

    public override void ContinueAnimation()
    {
        Debug.Log("enter wander state");
        stateMachine.animator.CrossFadeInFixedTime(moveHash, crossFadeDuration);
        stateMachine.animator.speed = stateMachine.baseSpeed;
        stateMachine.agent.speed = stateMachine.baseSpeed;
        stateMachine.agent.isStopped = false;
    }
}
