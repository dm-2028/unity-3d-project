using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : StateMachine
{
    //public Vector3 velocity;
    //public float movementSpeed { get; private set; } = 5f;
    //public float jumpForce { get; private set; } = 5f;
    public float lookRotationDampFactor { get; private set; } = 10f;

    //public Vector3 velocity, connectionVelocity, lastConnectionVelocity;

    public Transform mainCamera { get; private set; }
    public InputReader inputReader { get; private set; }
    public Animator animator { get; private set; }

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
        waterMask = -1;
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

    public Transform
        playerInputSpace = default,
        character = default;

    public float groundContactCount { get; set; }
    public float steepContactCount { get; set; }
    public float climbContactCount { get; set; }

    public int jumpPhase { get; set; }
    public int stepsSinceLastGrounded{get; set;}
    public int stepsSinceLastJump { get; set; }

    public Vector3 contactNormal { get; set; }
    public Vector3 steepNormal { get; set; }
    public Vector3 climbNormal { get; set; }
    public Vector3 lastClimbNormal { get; set; }
    public Vector3 lastContactNormal { get; set; }
    public Vector3 lastSteepNormal { get; set; }

    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

    public Rigidbody body;

    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main.transform;
        inputReader = GetComponent<InputReader>();
        animator = GetComponent<Animator>();
        
        SwitchState(new PlayerMoveState(this));
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = character.GetComponent<MeshRenderer>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

}
