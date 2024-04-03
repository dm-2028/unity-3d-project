using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;

    public void SwitchState(State state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }
    // Update is called once per frame
    public virtual void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.TickFixed();
    }

}
