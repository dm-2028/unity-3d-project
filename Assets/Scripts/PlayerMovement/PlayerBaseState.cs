using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    // Start is called before the first frame update

    protected bool desiredJump, desiresClimbing;
    protected Vector3 playerInput;
    protected Vector3 velocity, connectionVelocity, lastConnectionVelocity;
    protected Vector3 upAxis, rightAxis, forwardAxis;
    protected float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;

    protected Vector3 connectionWorldPosition, connectionLocalPosition;

    protected MeshRenderer meshRenderer;

    protected float submergence;

    protected bool OnGround => stateMachine.groundContactCount > 0;

    protected bool OnSteep => stateMachine.steepContactCount > 0;

    protected bool Climbing => stateMachine.climbContactCount > 0 && stateMachine.stepsSinceLastJump > 2;

    protected bool InWater => submergence > 0f;

    protected bool Swimming => submergence >= stateMachine.swimThreshold;

    protected Rigidbody connectedBody, previousConnectedBody;

    public float offsetFromWall = 0.3f;
    protected Transform helper = new GameObject().transform;
    protected LayerMask ignoreLayers;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        helper.name = "Climb Helper";
        this.stateMachine = stateMachine;

    }


    protected float GetMinDot(int layer)
    {
        return (stateMachine.stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    protected Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    protected void CalculateMoveDirection()
    {
        //Vector3 cameraForward = new(stateMachine.mainCamera.forward.x, 0, stateMachine.mainCamera.forward.z);
        //Vector3 cameraRight = new(stateMachine.mainCamera.right.x, 0, stateMachine.mainCamera.right.z);

        playerInput.x = stateMachine.inputReader.movement.x;
        playerInput.z = stateMachine.inputReader.movement.y;
        playerInput.y = 0f;
        Debug.Log("move direction: " + playerInput.ToString());
        if (stateMachine.playerInputSpace)
        {
            rightAxis = ProjectDirectionOnPlane(stateMachine.playerInputSpace.right, upAxis);
            forwardAxis =
                ProjectDirectionOnPlane(stateMachine.playerInputSpace.forward, upAxis);
        }
        else
        {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }
    }

    //protected void FaceMoveDirection()
    //{
    //    Vector3 faceDirection = new(stateMachine.velocity.x, 0f, stateMachine.velocity.z);

    //    if (faceDirection == Vector3.zero) return;

    //    stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(faceDirection), stateMachine.lookRotationDampFactor * Time.deltaTime);

    //}

    //protected void ApplyGravity()
    //{
    //    if(stateMachine.velocity.y > Physics.gravity.y)
    //    {
    //        stateMachine.velocity.y += Physics.gravity.y * Time.deltaTime;
    //    }
    //}

    //protected void Move()
    //{
    //    stateMachine.controller.Move(stateMachine.velocity * Time.deltaTime);
    //}

    protected void AdjustVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;

        if (Climbing)
        {
            acceleration = stateMachine.maxClimbAcceleration;
            speed = stateMachine.maxClimbSpeed;
            xAxis = Vector3.Cross(stateMachine.contactNormal, upAxis);
            zAxis = upAxis;
        }
        else if (InWater)
        {
            float swimFactor = Mathf.Min(1f, submergence / stateMachine.swimThreshold);
            acceleration = Mathf.LerpUnclamped(
                OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration, stateMachine.maxSwimAcceleration, swimFactor);
            speed = Mathf.LerpUnclamped(stateMachine.maxSpeed, stateMachine.maxSwimSpeed, swimFactor);
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        else
        {
            acceleration = OnGround ? stateMachine.maxAcceleration : stateMachine.maxAirAcceleration;
            speed = OnGround && desiresClimbing ? stateMachine.maxClimbSpeed : stateMachine.maxSpeed;
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, stateMachine.contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, stateMachine.contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;

        if (Swimming)
        {
            velocity += upAxis * adjustment.y;
        }
    }

    protected bool CheckSteepContacts()
    {
        if (stateMachine.steepContactCount > 1)
        {
            stateMachine.steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, stateMachine.steepNormal);
            if (upDot >= minGroundDotProduct)
            {
                stateMachine.groundContactCount = 1;
                stateMachine.contactNormal = stateMachine.steepNormal;
                return true;
            }
        }
        return false;
    }

    protected bool CheckClimbing()
    {
        if (Climbing)
        {
            if (stateMachine.climbContactCount > 1)
            {
                stateMachine.climbNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, stateMachine.climbNormal);
                if (upDot >= minGroundDotProduct)
                {
                    stateMachine.climbNormal = stateMachine.lastClimbNormal;
                }
            }
            stateMachine.groundContactCount = stateMachine.climbContactCount;
            stateMachine.contactNormal = stateMachine.climbNormal;
            return true;
        }
        return false;
    }

    protected bool CheckSwimming()
    {
        if (Swimming)
        {
            stateMachine.groundContactCount = 0;
            stateMachine.contactNormal = upAxis;
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
        stateMachine.lastContactNormal = stateMachine.contactNormal;
        stateMachine.lastSteepNormal = stateMachine.steepNormal;
        lastConnectionVelocity = connectionVelocity;
        stateMachine.groundContactCount = 0;
        stateMachine.steepContactCount = 0;
        stateMachine.climbContactCount = 0;
        stateMachine.contactNormal = Vector3.zero;
        stateMachine.steepNormal = Vector3.zero;
        stateMachine.climbNormal = Vector3.zero;
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
                stateMachine.groundContactCount += 1;
                stateMachine.contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else
            {
                if (upDot > -0.01f)
                {
                    stateMachine.steepContactCount += 1;
                    stateMachine.steepNormal += normal;
                    if (stateMachine.groundContactCount == 0)
                    {
                        connectedBody = collision.rigidbody;
                    }
                }
                Debug.Log("climb mask " + stateMachine.climbMask.value + " " + layer);
                int layerPrint = 1 << layer;
                Debug.Log("climb layer " + layerPrint);
                if (desiresClimbing && upDot >= minClimbDotProduct && (stateMachine.climbMask & (1 << layer)) == 0)
                {
                    stateMachine.climbContactCount += 1;
                    stateMachine.climbNormal += normal;
                    stateMachine.lastClimbNormal = normal;
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
            stateMachine.body.position + upAxis * stateMachine.submergenceOffset,
            -upAxis,
            out RaycastHit hit,
            stateMachine.submergenceRange + 1f,
            stateMachine.waterMask,
            QueryTriggerInteraction.Collide))
            {
                submergence = 1f - hit.distance / stateMachine.submergenceRange;
            }
            else
            {
                submergence = 1f;
            }
            if (Swimming)
            {
                connectedBody = other.attachedRigidbody;
            }
        }
    }

    protected void UpdateConnectionState()
    {
        if (connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = stateMachine.body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }

    protected bool SnapToGround()
    {
        return false;
        if (stateMachine.stepsSinceLastGrounded > 1 || stateMachine.stepsSinceLastJump <= 2)
        {
            return false;
        }
        float speed = stateMachine.body.velocity.magnitude;
        if (speed > stateMachine.maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(stateMachine.body.position, -upAxis, out RaycastHit hit, stateMachine.probeDistance, stateMachine.probeMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
        Debug.Log("snapping to ground");
        stateMachine.groundContactCount = 1;
        stateMachine.contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }
    protected void FaceMoveDirection()
    {
        Vector3 faceDirection = new(stateMachine.body.velocity.x, 0f, stateMachine.body.velocity.z);
        if (faceDirection == Vector3.zero) return;
        stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, Quaternion.LookRotation(-faceDirection), stateMachine.lookRotationDampFactor * Time.deltaTime);
    }

    public void PreventSnapToGround()
    {
        stateMachine.stepsSinceLastJump = -1;
    }

}
