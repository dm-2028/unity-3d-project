using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormStateMachine : StateMachine, IHitboxResponder
{
    private readonly int appearHash = Animator.StringToHash("Appear");

    private const float crossFadeDuration = 0.1f;

    public Animator animator { get; private set; }
    public EnemySpawn enemySpawn { get; set; }

    [Range(0f, 5.0f)]
    public float cooldownTime = 4f;

    public bool inCooldown { get; set; } = false;
    public bool IsDead { get; set; } = false;
    public bool takingDamage { get; set; } = false;


    public GameObject player;
    public GameObject worm;
    public float seeDistance = 10f;
    public int health = 2;
    public float baseSpeed = 1f;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        hitbox = GetComponentInChildren<HitBox>();
        hitbox.UseResponder(this);

        animator.Play(appearHash);
    }
    void ResetCooldown()
    {
        inCooldown = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void ReceiveDamage()
    {
        if (IsDead) return;
        Debug.Log("Receiving damage " + health);
        health--;
        Debug.Log("health " + health);
        takingDamage = true;
        if (health <= 0)
        {
            IsDead = true;
            enemySpawn.incrementKilled(gameObject);
            //SwitchState(new EnemyDeadState(this));
        }
        else
        {
            animator.speed = baseSpeed * 1.5f;
            //animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
        }
    }

    public void ResetHurt()
    {
        takingDamage = false;
        ((EnemyBaseState)currentState)?.ContinueAnimation();
    }

    public void CollidedWith(Collider collider)
    {
        Debug.Log("hitbox collided with " + collider.ToString() + collider.gameObject.tag.ToString());
        if (collider.gameObject.CompareTag("Player"))
        {
            Debug.Log("hitbox attacking player");
            collider.gameObject.GetComponent<PlayerStateMachine>().ReceiveDamage();
        }
    }

    public override void ContinueAnimation()
    {
        ((WormBaseState)currentState)?.ContinueAnimation();
    }
}
