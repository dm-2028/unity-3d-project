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
}
