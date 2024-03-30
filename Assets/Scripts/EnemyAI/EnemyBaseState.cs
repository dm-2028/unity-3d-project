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
        Vector3 position = stateMachine.transform.position + new Vector3(0, .5f, 0);
        Vector3 rayToTarget = stateMachine.player.transform.position - position;
        float lookAngle = Vector3.Angle(stateMachine.transform.forward + new Vector3(0, .5f, 0), rayToTarget);
        if (lookAngle < 60 && Physics.Raycast(position, rayToTarget.normalized * stateMachine.seeDistance, out RaycastHit hit))
        {
            Debug.DrawRay(position, rayToTarget.normalized * stateMachine.seeDistance, Color.green);
            Debug.Log("raycast hitting " + hit.transform.gameObject.tag);
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        Debug.DrawRay(position, rayToTarget.normalized * stateMachine.seeDistance, Color.red);
        Debug.Log("can't see player");
        return false;
    }
}
