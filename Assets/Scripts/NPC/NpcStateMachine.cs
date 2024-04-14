using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    public Animator animator { get; private set; }

    public float baseSpeed = .5f;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SwitchState(new NpcIdleState(this));
    }

    public void ReceiveDamge()
    {

    }

    public void RotateTowardsplayer(Vector3 position)
    {
        StartCoroutine(_RotateTowardsPlayer(position));
    }

    IEnumerator _RotateTowardsPlayer(Vector3 position)
    {
        Quaternion lookRotation = Quaternion.LookRotation(transform.position-new Vector3(position.x, transform.position.y, position.z));

        Quaternion initialRotation = transform.rotation;
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);

            time += Time.deltaTime * 10f;

            yield return null;
        }
    }
}
