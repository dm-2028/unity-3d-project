using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected readonly EnemyStateMachine stateMachine;
    protected EnemyBaseState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected bool CanSeePlayer()
    {
        Vector3 rayToTarget = stateMachine.player.transform.position - stateMachine.transform.position;
        float lookAngle = Vector3.Angle(stateMachine.transform.forward, rayToTarget);

        if (lookAngle < 60 && Physics.Raycast(stateMachine.transform.position, rayToTarget.normalized * stateMachine.seeDistance, out RaycastHit hit))
        {
            if (hit.transform.gameObject.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }
}
