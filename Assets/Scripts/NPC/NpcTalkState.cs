using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcTalkState : NpcBaseState
{
    private readonly int talkHash = Animator.StringToHash("Talk");
    private const float animationDampTime = 0.1f;
    private const float crossFadeDuration = 0.1f;

    

    public NpcTalkState(NpcStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("enter talking state");
        ContinueAnimation();
        GameObject.FindGameObjectWithTag("Menu").GetComponent<MainUIManager>().DisplayText(stateMachine.dialog);
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
    public override void ContinueAnimation()
    {
        stateMachine.animator.speed = stateMachine.baseSpeed;
        stateMachine.animator.CrossFadeInFixedTime(talkHash, crossFadeDuration);

    }
}
