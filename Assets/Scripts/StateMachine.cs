using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State currentState;
    public bool isAttacking { get; set; } = false;
    public HitBox hitbox { get; set; }

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
        if (isAttacking)
        {
            hitbox.HitboxUpdate();
        }
    }

    private void FixedUpdate()
    {
        currentState?.TickFixed();
    }
    public virtual void ContinueAnimation()
    {

    }
}
