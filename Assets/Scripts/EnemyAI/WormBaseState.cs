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
        Vector3 position = stateMachine.transform.position;
        Vector3 rayToTarget = stateMachine.player.transform.position - position;
        float lookAngle = Vector3.Angle(stateMachine.transform.forward, rayToTarget);

        Quaternion lookRotation = Quaternion.LookRotation(rayToTarget.normalized);
        stateMachine.worm.transform.rotation = Quaternion.Slerp(stateMachine.worm.transform.rotation, lookRotation, 5f * Time.deltaTime);
    }
}
