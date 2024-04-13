using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcIdleState : NpcBaseState
{

    private readonly int idleHash = Animator.StringToHash("Idle");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    public NpcIdleState(NpcStateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        ContinueAnimation();
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }

    public override void TickFixed()
    {
    }

    public void ContinueAnimation()
    {
        stateMachine.animator.CrossFadeInFixedTime(idleHash, crossFadeDuration);
        stateMachine.animator.speed = stateMachine.baseSpeed;

    }
}
Fpl