using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RatBaseState : State
{
    protected readonly RatStateMachine stateMachine;
    protected RatBaseState(RatStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected bool CanSeePlayer()
    {
        Vector3 position = stateMachine.transform.position + new Vector3(0, .5f, 0);
        Vector3 rayToTarget = stateMachine.player.transform.position - position;
        float lookAngle = Vector3.Angle(stateMachine.transform.forward + new Vector3(0, .5f, 0), rayToTarget);
        if (lookAngle < 60 && Physics.Raycast(position, rayToTarget.normalized * stateMachine.seeDistance, out RaycastHit hit))
        {
            Debug.DrawRay(position, rayToTarget.normalized * stateMachine.seeDistance, Color.green);
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        Debug.DrawRay(position, rayToTarget.normalized * stateMachine.seeDistance, Color.red);
        return false;
    }
    public virtual void ContinueAnimation()
    {

    }
}
