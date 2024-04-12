using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    public Animator animator { get; private set; }

    public float baseSpeed = .5f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SwitchState(new NpcIdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamge()
    {

    }
}
