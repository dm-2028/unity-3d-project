using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcBaseState : State
{
    protected readonly NpcStateMachine stateMachine;
    protected NpcBaseState(NpcStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public virtual void ContinueAnimation()
    {

    }
}
