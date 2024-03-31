using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private readonly int attackHash = Animator.StringToHash("Attack");
    private const float crossFadeDuration = 0.1f;

    private HitBox hitbox;
    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.CrossFadeInFixedTime(attackHash, crossFadeDuration);
        stateMachine.hitbox.StartCheckingCollision();
    }

    public override void Exit()
    {
        stateMachine.hitbox.StopCheckingCollision();
    }

    public override void Tick()
    {
        if (!AnimatorIsPlaying())
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
        else
        {
            stateMachine.hitbox.hitboxUpdate();
        }
    }

    public override void TickFixed()
    {
    }

    bool AnimatorIsPlaying()
    {
        return stateMachine.animator.GetCurrentAnimatorStateInfo(0).length >
               stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
