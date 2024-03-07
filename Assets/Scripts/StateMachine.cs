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
        Debug.Log("on collision enter");
        currentState?.EvaluateCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        currentState?.EvaluateCollision(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        currentState?.ExitCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {

        currentState?.EvaluateSubmergence(other);

    }

    private void OnTriggerStay(Collider other)
    {
        currentState?.EvaluateSubmergence(other);  
    }
}
