using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private State currentState;

    public void SwitchState(State state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }
    // Update is called once per frame
    void Update()
    {
        currentState?.Tick();
    }

    private void FixedUpdate()
    {
        currentState?.TickFixed();
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState?.EvaluateCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        currentState?.EvaluateCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            currentState?.EvaluateSubmergence(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            currentState?.EvaluateSubmergence(other);
        }
    }
}
