using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("Movement");
    private readonly int moveHash = Animator.StringToHash("Move");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    private float seeCooldown = 0;

    public EnemyPursueState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        ContinueAnimation();
    }

    public override void Tick()
    {
        if (!stateMachine.takingDamage)
        {
            float distance = (stateMachine.transform.position - stateMachine.player.transform.position).magnitude;
            if (distance > 1)
            {
                stateMachine.agent.isStopped = false;
                stateMachine.agent.SetDestination(stateMachine.player.transform.position);
                //Debug.Log(stateMachine.agent.destination + " Sis the destination");
            }
            else
            {
                Debug.Log("switch state attack");
                stateMachine.SwitchState(new EnemyAttackState(stateMachine));
            }
            if (!CanSeePlayer())
            {
                seeCooldown += Time.fixedDeltaTime;
                Debug.Log("switch state wander");
                if (seeCooldown > 5)
                {
                    stateMachine.SwitchState(new EnemyWanderState(stateMachine));
                }
            }
            else
            {
                seeCooldown = 0;
            }
        }
    }
    public override void TickFixed()
    {

    }

    public override void Exit()
    {
    }

    public override void ContinueAnimation()
    {
        stateMachine.animator.CrossFadeInFixedTime(moveHash, crossFadeDuration);
        stateMachine.animator.speed = stateMachine.baseSpeed*1.5f;
        stateMachine.agent.speed = stateMachine.baseSpeed*1.5f;
        stateMachine.agent.isStopped = false;
    }
}
