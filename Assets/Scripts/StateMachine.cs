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
        Debug.Log("on trigger enter " + other.gameObject.tag);
        if (other.gameObject.tag == ("Collectable"))
        {
            Debug.Log("trigger is tagged");
            Collectable collectable = other.gameObject.GetComponent<Collectable>();
            StartCoroutine(PullCollectable(collectable));
        }
        else
        {
            currentState?.EvaluateSubmergence(other);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        currentState?.EvaluateSubmergence(other);  
    }

    IEnumerator PullCollectable(Collectable collectable)
    {
        Debug.Log("start coroutine");
        float i = 0f;
        float rate = 1.0f / 3.0f;
            
        while(transform.position != collectable.transform.position)
        {
            i += Time.deltaTime / rate;
            collectable.transform.position = Vector3.Lerp(collectable.transform.position, transform.position, i);
            yield return null;
        }
        collectable.collected = true;
        collectable.gameObject.SetActive(false);
    }
}
