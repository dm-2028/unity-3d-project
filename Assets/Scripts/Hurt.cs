using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt : MonoBehaviour
{
    EnemyStateMachine esm;
    
    // Start is called before the first frame update
    void Start()
    {
        esm = GetComponentInParent<EnemyStateMachine>();
    }

    // Update is called once per frame
    public void ResetHurt()
    {
        esm.ResetHurt();
    }
}
