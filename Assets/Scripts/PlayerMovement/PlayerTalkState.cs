using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkState : PlayerBaseState
{
    private readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    GameObject subject;

    bool facingSubject = false;

    Quaternion lookRotation;

    public PlayerTalkState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        ContinueAnimation();
        subject = stateMachine.nearbyNPC;
        subject.GetComponent<NpcStateMachine>().RotateTowardsplayer(stateMachine.transform.position);
        lookRotation = Quaternion.LookRotation(stateMachine.transform.position- new Vector3(subject.transform.position.x, stateMachine.transform.position.y, subject.transform.position.z));
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {


    }

    public override void TickFixed()
    {
        if (!facingSubject)
        {
            stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, lookRotation, Time.deltaTime*5f);
            float dot = Vector3.Dot(stateMachine.transform.forward, (subject.transform.position - stateMachine.transform.position).normalized);
            if(dot > .95)
            {
                facingSubject = true;
            }
        }
    }

    public override void ContinueAnimation()
    {
        stateMachine.animator.speed = stateMachine.baseAnimationSpeed;
        stateMachine.animator.CrossFadeInFixedTime(moveBlendTreeHash, crossFadeDuration);
    }
}
