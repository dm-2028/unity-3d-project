using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    // Start is called before the first frame update

    int jumpPhase, stepsSinceLastGrounded, stepsSinceLastJump;
    float groundContactCount, steepContactCount, climbContactCount;
    bool desiredJump, desiresClimbing;
    Vector3 playerInput;
    Vector3 velocity, connectionVelocity, lastConnectionVelocity;
    Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal, lastContactNormal, lastSteepNormal;
    protected Vector3 upAxis, rightAxis, forwardAxis;
    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

    Vector3 connectionWorldPosition, connectionLocalPosition;

    MeshRenderer meshRenderer;

    float submergence;

    bool OnGround => groundContactCount > 0;

    bool OnSteep => steepContactCount > 0;

    bool Climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

    bool InWater => submergence > 0f;

    bool Swimming => submergence >= stateMachine.swimThreshold;

    Rigidbody connectedBody, previousConnectedBody;

    public float offsetFromWall = 0.3f;
    protected Transform helper = new GameObject().transform;
    protected LayerMask ignoreLayers;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        helper.name = "Climb Helper";
        this.stateMachine = stateMachine;

    }

    public override void TickFixed()
    {
        upAxis = -Physics.gravity.normalized;
        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        if (InWater)
        {
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        }
        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump(gravity);
        }
        if (Climbing)
        {
            velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
        }
        else if (InWater)
        {
            velocity += gravity * ((1f - bouyancy * submergence) * Time.deltaTime);
        }
        else if (OnGround && velocity.sqrMagnitude < 0.01f)
        {
            velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        }
        else if (desiresClimbing && OnGround)
        {
            velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
        }
        body.velocity = velocity;
        ClearState();
    }

    protected void CalculateMoveDirection()
    {
        Vector3 cameraForward = new(stateMachine.mainCamera.forward.x, 0, stateMachine.mainCamera.forward.z);
        Vector3 cameraRight = new(stateMachine.mainCamera.right.x, 0, stateMachine.mainCamera.right.z);

        Vector3 moveDirection;
        moveDirection.x = stateMachine.inputReader.movement.x;
        moveDirection.z = stateMachine.inputReader.movement.y;
        if (playerInputSpace)
        {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
            forwardAxis =
                ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        }
        else
        {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }
    }

    protected void FaceMoveDirection()
    {
        Vector3 faceDirection = new(stateMachine.velocity.x, 0f, stateMachine.velocity.z);

        if (faceDirection == Vector3.zero) return;

        stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(faceDirection), stateMachine.lookRotationDampFactor * Time.deltaTime);

    }

    protected void ApplyGravity()
    {
        if(stateMachine.velocity.y > Physics.gravity.y)
        {
            stateMachine.velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }
    
    protected void Move()
    {
        stateMachine.controller.Move(stateMachine.velocity * Time.deltaTime);
    }

    bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

    bool CheckClimbing()
    {
        if (Climbing)
        {
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, climbNormal);
                if (upDot >= minGroundDotProduct)
                {
                    climbNormal = lastClimbNormal;
                }
            }
            groundContactCount = climbContactCount;
            contactNormal = climbNormal;
            return true;
        }
        return false;
    }

    bool CheckSwimming()
    {
        if (Swimming)
        {
            groundContactCount = 0;
            contactNormal = upAxis;
            return true;
        }
        return false;
    }

    protected Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }

    protected void ClearState()
    {
        lastContactNormal = contactNormal;
        lastSteepNormal = steepNormal;
        lastConnectionVelocity = connectionVelocity;
        groundContactCount = 0;
        steepContactCount = 0;
        climbContactCount = 0;
        contactNormal = Vector3.zero;
        steepNormal = Vector3.zero;
        climbNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
        submergence = 0f;
    }

    public override void EvaluateCollision(Collision collision)
    {
        if (Swimming)
        {
            return;
        }
        int layer = collision.gameObject.layer;

        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else
            {
                if (upDot > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0)
                    {
                        connectedBody = collision.rigidbody;
                    }
                }
                Debug.Log("climb mask " + climbMask.value + " " + layer);
                int layerPrint = 1 << layer;
                Debug.Log("climb layer " + layerPrint);
                if (desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) == 0)
                {
                    climbContactCount += 1;
                    climbNormal += normal;
                    lastClimbNormal = normal;
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }

    public override void EvaluateSubmergence(Collider other)
    {
        if ((stateMachine.waterMask & (1 << other.gameObject.layer)) != 0)
        {
            if (Physics.Raycast(
            body.position + upAxis * submergenceOffset,
            -upAxis,
            out RaycastHit hit,
            submergenceRange + 1f,
            waterMask,
            QueryTriggerInteraction.Collide))
            {
                submergence = 1f - hit.distance / submergenceRange;
            }
            else
            {
                submergence = 1f;
            }
            if (Swimming)
            {
                connectedBody = collider.attachedRigidbody;
            }
        }
    }
}
