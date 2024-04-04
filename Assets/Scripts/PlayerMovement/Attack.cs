using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    StateMachine stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = GetComponentInParent<StateMachine>();
    }

    public void EndAttack()
    {
        Debug.Log("ending attack");
        stateMachine.isAttacking = false;
        stateMachine.hitbox.StopCheckingCollision();
        stateMachine.ContinueAnimation();

    }

    public void StartHitbox()
    {
        Debug.Log("start collision");
        stateMachine.hitbox.StartCheckingCollision();
    }
}
