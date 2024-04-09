using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class PlayerStateMachine : StateMachine, IHitboxResponder
{
    //public Vector3 velocity;
    //public float movementSpeed { get; private set; } = 5f;
    //public float jumpForce { get; private set; } = 5f;
    public float lookRotationDampFactor { get; private set; } = 10f;

    //public Vector3 velocity, connectionVelocity, lastConnectionVelocity;

    public Transform mainCamera { get; private set; }
    public InputReader inputReader { get; private set; }
    public Animator animator { get; private set; }

    public GameManager gameManager;

    [Range(0f, 100f)]
    public float
        maxSpeed = 10f,
        maxClimbSpeed = 2f,
        maxSwimSpeed = 5f;
    [Range(0f, 100f)]
    public float
        maxAcceleration = 10f,
        maxAirAcceleration = 1f,
        maxClimbAcceleration = 20f,
        maxSwimAcceleration = 5f;
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    [Range(0, 5)]
    public int maxAirJumps = 0;
    [Range(0f, 90f)]
    public float
        maxGroundAngle = 25,
        maxStairsAngle = 50f;
    [Range(0f, 100f)]
    public float maxSnapSpeed = 100f;
    [Min(0f)]
    public float probeDistance = 1f;

    public LayerMask
        probeMask = -1,
        stairsMask = -1,
        climbMask = -1,
        waterMask = -1,
        pitMask = -1;
    [Range(90, 180)]
    public float maxClimbAngle = 140f;

    public float submergenceOffset = 0.5f;
    [Min(0.1f)]
    public float submergenceRange = 1f;
    [Range(0f, 10f)]
    public float waterDrag = 1f;
    [Min(0f)]
    public float bouyancy = 1f;
    [Range(0.01f, 1f)]
    public float swimThreshold = 0.5f;
    [Min(0.1f)]
    public float ballRadius = 0.5f;
    [Min(0f)]
    public float ballAlignSpeed = 180f;
    [Min(0f)]
    public float
        ballAirRotation = 0.5f,
        ballSwimRotation = 2f;
    public float baseAnimationSpeed = 1;

    public Transform
        playerInputSpace = default,
        character = default;

    public float groundContactCount { get; set; }
    public float steepContactCount { get; set; }
    public float climbContactCount { get; set; }

    public int jumpPhase { get; set; }
    public int stepsSinceLastGrounded { get; set; }
    public int stepsSinceLastJump { get; set; }

    public Vector3 contactNormal { get; set; }
    public Vector3 steepNormal { get; set; }
    public Vector3 climbNormal { get; set; }
    public Vector3 lastClimbNormal { get; set; }
    public Vector3 lastContactNormal { get; set; }
    public Vector3 lastSteepNormal { get; set; }

    public Vector3 velocity {get; set;}
    public Vector3 connectionVelocity { get; set; }
    public Vector3 lastConnectionVelocity { get; set; }
    public Vector3 upAxis;
    public Vector3 rightAxis;
    public Vector3 forwardAxis;
    public Vector3 connectionWorldPosition { get; set; }
    public Vector3 connectionLocalPosition { get; set; }
    public Vector3 lastGroundPosition { get; set; }
    public Quaternion lastGroundRotation { get; set; }
    public float minGroundDotProduct { get; private set; }
    public float minStairsDotProduct { get; private set; }
    public float minClimbDotProduct { get; private set; }
    public float submergence { get; set; }

    public bool jumpFromSwim { get; set; }
    public bool isGrounded { get; set; }

    public Rigidbody connectedBody { get; set; }
    public Rigidbody previousConnectedBody { get; set; }

    public Rigidbody body;

    MeshRenderer meshRenderer;

    private int health = 10;
    private int maxHealth = 10;

    private float damageCooldown = 2f;


    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      
        mainCamera = Camera.main.transform;
        inputReader = GetComponent<InputReader>();
        animator = GetComponentInChildren<Animator>();
        hitbox = GetComponentInChildren<HitBox>();
        hitbox.UseResponder(this);
        
        SwitchState(new PlayerMoveState(this));
    }


    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = character.GetComponent<MeshRenderer>();
        OnValidate();
    }

    public void DecrementHealth(int damage)
    {
        health -= damage;
        gameManager.UpdateHealth(health);
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("on collision enter");
        ((PlayerBaseState)currentState)?.EvaluateCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        ((PlayerBaseState)currentState)?.EvaluateCollision(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        ((PlayerBaseState)currentState)?.ExitCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.transform.parent.tag == ("Collectable"))
        {
            StartCoroutine(PullCollectable(other.transform.parent.gameObject));
        }
        else
        {
            ((PlayerBaseState)currentState)?.EvaluateSubmergence(other);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        ((PlayerBaseState)currentState)?.EvaluateSubmergence(other);
    }

    IEnumerator PullCollectable(GameObject collectable)
    {
        Debug.Log("start coroutine");
        float i = 0f;
        float rate = 1.0f / 3.0f;

        while (transform.position != collectable.transform.position)
        {
            i += Time.deltaTime / rate;
            collectable.transform.position = Vector3.Lerp(collectable.transform.position, transform.position, i);
            yield return null;
        }
        Collectable bean = collectable.transform.GetComponent<Collectable>();
        collectable.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        bean.collected = true;
        MainManager.Instance.coffeeBeanCollected[bean.serializationId] = true;
        gameManager.IncrementBeans();
        GameManager.SaveData();
    }

    public void CollidedWith(Collider collider)
    {
        Debug.Log("hitbox collided with " + collider.ToString() + collider.gameObject.tag.ToString());
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("hitbox attacking enemy");
            collider.gameObject.GetComponentInParent<EnemyStateMachine>().ReceiveDamage();
        }
    }

    public override void ContinueAnimation()
    {
        ((PlayerBaseState)currentState)?.ContinueAnimation();
    }

    public void ReceiveDamage()
    {

        if (health <= 0)
        {
            //player is dead
            //isDead = true;
            //enemySpawn.incrementKilled(gameObject);
            //SwitchState(new EnemyDeadState(this));
        }
        else
        {
            DecrementHealth(1);
        }
    }
}
