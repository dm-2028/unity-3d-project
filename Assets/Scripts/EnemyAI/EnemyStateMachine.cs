using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateMachine : StateMachine
{
    protected const float crossFadeDuration = 0.1f;

    public Animator animator { get; protected set; }
    [Range(0f, 5.0f)]
    public float cooldownTime = 1f;

    public bool inCooldown { get; set; } = false;
    public bool IsDead { get; set; } = false;
    public bool takingDamage { get; set; } = false;


    public GameObject player;
    public float seeDistance = 10f;
    public int health = 2;
    public float baseSpeed = 1f;

    void ResetCooldown()
    {
        inCooldown = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public abstract void ResetHurt();

    public abstract void ReceiveDamage(int damage);

    public void CollidedWith(Collider collider)
    {
        Debug.Log("hitbox collided with " + collider.ToString() + collider.gameObject.tag.ToString());
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("hitbox attacking player");
            collider.gameObject.GetComponent<PlayerStateMachine>().ReceiveDamage(1);
        }
    }
}
