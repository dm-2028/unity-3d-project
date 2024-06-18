using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormStateMachine : EnemyStateMachine, IHitboxResponder
{
    private readonly int appearHash = Animator.StringToHash("Appear");
    private readonly int hitHash = Animator.StringToHash("WormHurt");

    public EnemySpawn enemySpawn { get; set; }

    public GameObject worm;
    public GameObject slamAttack;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponentInChildren<Animator>();
        hitbox = GetComponentInChildren<HitBox>();
        hitbox.UseResponder(this);
        animator.Play(appearHash);
    }

    public override void ReceiveDamage(int damage)
    {
        if (IsDead) return;
        Debug.Log("Receiving damage " + health);
        health -= damage;
        Debug.Log("health " + health);
        takingDamage = true;
        if (health <= 0)
        {
            IsDead = true;
            //enemySpawn.incrementKilled(gameObject);
            SwitchState(new WormDeadState(this));
        }
        else
        {
            animator.speed = baseSpeed * 1.5f;
            animator.CrossFadeInFixedTime(hitHash, crossFadeDuration);
        }
    }

    public override void ResetHurt()
    {
        takingDamage = false;
        ((WormBaseState)currentState)?.ContinueAnimation();
    }


    public override void ContinueAnimation()
    {
        ((WormBaseState)currentState)?.ContinueAnimation();
    }
    public IEnumerator SlamAttack()
    {
        Vector3 slamPosition = transform.position + worm.transform.up;
        PlayerStateMachine psm = player.GetComponent<PlayerStateMachine>();
        GameObject _slam = Instantiate(slamAttack, transform.position + worm.transform.up + new Vector3(0, .01f, 0), Quaternion.identity);
        Renderer renderer = _slam.GetComponent<Renderer>();

        float innerRadius = 0;
        float outerRadius = .25f;
        float maxRadius = renderer.bounds.extents.x;
        float movementSpeed = 2f;
        Debug.Log("renderer size" + maxRadius);
        while(outerRadius < maxRadius)
        {
            renderer.material.SetFloat("_Timer", innerRadius * .051f);
            Vector3 distance = player.transform.position - slamPosition;
            float magnitude = distance.magnitude;
            Debug.Log("inner radius " + innerRadius + " outerradius " + outerRadius + " " + magnitude);

            if (magnitude < outerRadius && magnitude > innerRadius && psm.isGrounded)
            {
                psm.ReceiveDamage(1);
            }
            Vector3 point = slamPosition + (distance.normalized * innerRadius);
            Vector3 pointTwo = slamPosition + (distance.normalized * outerRadius);

            Debug.DrawLine(point, pointTwo, Color.red, .01f);

            innerRadius += Time.deltaTime*movementSpeed;
            outerRadius += Time.deltaTime*movementSpeed;
            yield return null;
        }
        GameObject.Destroy(_slam);

    }
}
