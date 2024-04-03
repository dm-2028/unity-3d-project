using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    PlayerStateMachine psm;

    // Start is called before the first frame update
    void Start()
    {
        psm = GetComponentInParent<PlayerStateMachine>();
    }

    public void EndAttack()
    {
        Debug.Log("ending attack");
        psm.isAttacking = false;
        psm.hitbox.StopCheckingCollision();
        psm.ContinueAnimation();

    }

    public void StartHitbox()
    {
        Debug.Log("start collision");
        psm.hitbox.StartCheckingCollision();
    }
}
