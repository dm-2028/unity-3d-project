using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WormBaseState : State
{
    protected readonly WormStateMachine stateMachine;
    protected WormBaseState(WormStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void RotateTowardsPlayer()
    {
        Vector3 position = stateMachine.transform.position + new Vector3(0, .5f, 0);
        Vector3 rayToTarget = stateMachine.player.transform.position - position;
        rayToTarget.y = 0;
        float lookAngle = Vector3.Angle(stateMachine.transform.forward, rayToTarget);
        Debug.DrawRay(position, rayToTarget * stateMachine.seeDistance, Color.green);
        Quaternion lookRotation = Quaternion.LookRotation(rayToTarget)*Quaternion.Euler(-90, 180, 0);
        stateMachine.worm.transform.rotation = Quaternion.Slerp(stateMachine.worm.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public virtual void ContinueAnimation()
    {

    }
}
