using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonoBehaviour
{
    WormStateMachine wsm;
    // Start is called before the first frame update
    void Start()
    {
        wsm = GetComponentInParent<WormStateMachine>();
    }

    public void StartIdleState()
    {
        wsm.SwitchState(new WormIdleState(wsm));
    }

    public void StartSlamAttack()
    {
        wsm.StartCoroutine("SlamAttack");
    }
}
