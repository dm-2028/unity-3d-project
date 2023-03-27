using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    private GameObject attached;

    public PlayerClimbState(PlayerStateMachine stateMachine, GameObject attached): base(stateMachine)
    {
        this.attached = attached;
    }
    public override void Enter()
    { 
        Debug.Log("enter climb state");
        stateMachine.transform.position = attached.transform.position+new Vector3(0,0,.1f);
    }

    public override void Exit()
    {
       
    }

    public override void Tick()
    {
        stateMachine.transform.position = attached.transform.position;
        attached.GetComponent<Rigidbody>().AddForce(stateMachine.inputReader.movement);
    }

}
