using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;

    protected bool jumping;

    protected Vector3 playerInput;

    protected bool OnGround => stateMachine.groundContactCount > 0;

    protected bool OnSteep => stateMachine.steepContactCount > 0;

    protected bool Climbing => stateMachine.climbContactCount > 0 && stateMachine.stepsSinceLastJump > 2;

    protected bool InWater => stateMachine.submergence > 0f;

    protected bool Swimming => stateMachine.submergence >= stateMachine.swimThreshold;


    public float offsetFromWall = 0.3f;

    protected List<ContactPoint> climbNormals = new List<ContactPoint>();

    private readonly int attackHash = Animator.StringToHash("Attack");
    private const float crossFadeDuration = 0.1f;


    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

    }


    protected float GetMinDot(int layer)
    {
        return (stateMachine.stairsMask & (1 << layer)) == 0 ? stateMachine.minGroundDotProduct : stateMachine.minStairsDotProduct;
    }

    protected Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    protected void CalculateMoveDirection()
    {
        const float e = 0.01f;
        if(stateMachine.inputReader.movement.x > e || stateMachine.inputReader.movement.x < -e)
        {
            playerInput.x = stateMachine.inputReader.movement.x;
        }
        else
        {
            playerInput.x = 0;
        }

        if (stateMachine.inputReader.movement.y > e || stateMachine.inputReader.movement.y < -e)
        {
            playerInput.z = stateMachine.inputReader.movement.y;
        }
        else
        {
            playerInput.z = 0;
        }
        playerInput.y = 0f;

        playerInput = Vector3.ClampMagnitude(playerInput, 1f);
        if (stateMachine.playerInputSpace)
        {
            stateMachine.rightAxis = ProjectDirectionOnPlane(stateMachine.playerInputSpace.right, stateMachine.upAxis);
            stateMachine.forwardAxis =
                ProjectDirectionOnPlane(stateMachine.playerInputSpace.forward, stateMachine.upAxis);
        }
        else
        {
            stateMachine.rightAxis = ProjectDirectionOnPlane(Vector3.right, stateMachine.upAxis);
            stateMachine.forwardAxis = ProjectDirectionOnPlane(Vector3.forward, stateMachine.upAxis);
        }
    }
    
    protected void UpdateState()
    {
        stateMachine.stepsSinceLastGrounded += 1;
        stateMachine.stepsSinceLastJump += 1;
        stateMachine.velocity = stateMachine.body.velocity;
            
        if(OnGround && !OnSteep)
        {
            SetPosition();
        }
        if (CheckClimbing())
        {
            stateMachine.SwitchState(new PlayerClimbState(stateMachine));
        }else if(CheckSwimming() && !jumping){
            stateMachine.SwitchState(new PlayerSwimState(stateMachine));
        }
        if (!jumping && (OnGround || SnapToGround() || CheckSteepContacts()))
        {
            stateMachine.stepsSinceLastGrounded = 0;
            if (stateMachine.stepsSinceLastJump > 1)
            {
                stateMachine.jumpPhase = 0;
            }
            if (stateMachine.groundContactCount > 1)
            {
                stateMachine.contactNormal.Normalize();
            }
        }
        else
        {
            stateMachine.contactNormal = stateMachine.upAxis;
        }

        if (stateMachine.connectedBody)
        {
            if (stateMachine.connectedBody.isKinematic || stateMachine.connectedBody.mass >= stateMachine.body.mass)
            {
                UpdateConnectionState();
            }
        }
    }

    protected bool CheckSteepContacts()
    {
        if (stateMachine.steepContactCount > 1)
        {
            stateMachine.steepNormal.Normalize();
            float upDot = Vector3.Dot(stateMachine.upAxis, stateMachine.steepNormal);
            if (upDot >= stateMachine.minGroundDotProduct)
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
                float upDot = Vector3.Dot(stateMachine.upAxis, stateMachine.climbNormal);
                if (upDot >= stateMachine.minGroundDotProduct)
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
        //Debug.Log("check swimming " + Swimming);
        if (Swimming)
        {
            stateMachine.groundContactCount = 0;
            stateMachine.contactNormal = stateMachine.upAxis;
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
        stateMachine.isGrounded = OnGround;
        stateMachine.lastContactNormal = stateMachine.contactNormal;
        stateMachine.lastSteepNormal = stateMachine.steepNormal;
        stateMachine.lastConnectionVelocity = stateMachine.connectionVelocity;
        stateMachine.groundContactCount = 0;
        stateMachine.steepContactCount = 0;
        stateMachine.climbContactCount = 0;
        stateMachine.contactNormal = Vector3.zero;
        stateMachine.steepNormal = Vector3.zero;
        stateMachine.climbNormal = Vector3.zero;
        stateMachine.connectionVelocity = Vector3.zero;
        stateMachine.previousConnectedBody = stateMachine.connectedBody;
        stateMachine.connectedBody = null;
        stateMachine.submergence = 0f;
        climbNormals.Clear();
    }

    public void EvaluateCollision(Collision collision)
    {
        if (Swimming)
        {
            return;
        }
        int layer = collision.gameObject.layer;

        float minDot = GetMinDot(layer);
        

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint point = collision.GetContact(i);
            Vector3 normal = point.normal;
            bool alreadyEvaluated = false;
            foreach (ContactPoint contactPoint in climbNormals)
            {
                if (normal.Equals(collision.GetContact(i).normal))
                {
                    alreadyEvaluated = true;
                    break;
                }
            }
            if (alreadyEvaluated)
            {
                continue;
            }

            climbNormals.Add(point);
            float upDot = Vector3.Dot(stateMachine.upAxis, normal);
            if (upDot >= minDot)
            {
                stateMachine.groundContactCount += 1;
                stateMachine.contactNormal += normal;
                stateMachine.connectedBody = collision.rigidbody;
            }
            else
            {
                if (upDot > -0.01f)
                {
                    stateMachine.steepContactCount += 1;
                    stateMachine.steepNormal += normal;
                    if (stateMachine.groundContactCount == 0)
                    {
                        stateMachine.connectedBody = collision.rigidbody;
                    }
                }
                if (upDot >= stateMachine.minClimbDotProduct && (stateMachine.climbMask & (1 << layer)) == 0)
                {
                    stateMachine.climbContactCount += 1;
                    stateMachine.climbNormal += normal;
                    stateMachine.lastClimbNormal = normal;
                    stateMachine.connectedBody = collision.rigidbody;
                }
            }
        }
    }

    public void ExitCollision(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if((stateMachine.climbMask & (1 << layer)) == 0)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }
    }

    public void EvaluateSubmergence(Collider other)
    {

        if ((stateMachine.pitMask & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("layer is pit layer");
            stateMachine.transform.position = stateMachine.lastGroundPosition;
            stateMachine.body.velocity = new(0f, 0f, 0f);
            stateMachine.DecrementHealth(1);
            return;
        }
        else if ((stateMachine.waterMask & (1 << other.gameObject.layer)) != 0)
        {
            if (Physics.Raycast(
            stateMachine.body.position + stateMachine.upAxis * stateMachine.submergenceOffset,
            -stateMachine.upAxis,
            out RaycastHit hit,
            stateMachine.submergenceRange + 1f,
            stateMachine.waterMask,
            QueryTriggerInteraction.Collide))
            {
                stateMachine.submergence = 1f - hit.distance / stateMachine.submergenceRange;
            }
            else
            {
                stateMachine.submergence = 1f;
            }
            if (Swimming)
            {
                stateMachine.connectedBody = other.attachedRigidbody;
            }
        }
    }

    protected void UpdateConnectionState()
    {
        if (stateMachine.connectedBody == stateMachine.previousConnectedBody)
        {
            Vector3 connectionMovement = stateMachine.connectedBody.transform.TransformPoint(stateMachine.connectionLocalPosition) - stateMachine.connectionWorldPosition;
            stateMachine.connectionVelocity = connectionMovement / Time.deltaTime;
        }
        stateMachine.connectionWorldPosition = stateMachine.body.position;
        stateMachine.connectionLocalPosition = stateMachine.connectedBody.transform.InverseTransformPoint(stateMachine.connectionWorldPosition);
    }

    protected bool SnapToGround()
    {
        if (stateMachine.stepsSinceLastGrounded > 1 || stateMachine.stepsSinceLastJump <= 2)
        {
            return false;
        }
        float speed = stateMachine.body.velocity.magnitude;
        if (speed > stateMachine.maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(stateMachine.body.position, -stateMachine.upAxis, out RaycastHit hit, stateMachine.probeDistance, stateMachine.probeMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        float upDot = Vector3.Dot(stateMachine.upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }
        Debug.Log("snapping to ground");
        stateMachine.groundContactCount = 1;
        stateMachine.contactNormal = hit.normal;
        float dot = Vector3.Dot(stateMachine.velocity, hit.normal);
        if (dot > 0f)
        {
            stateMachine.velocity = (stateMachine.velocity - hit.normal * dot).normalized * speed;
        }
        stateMachine.connectedBody = hit.rigidbody;
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

    protected void CheckDoubleJump()
    {
        if (stateMachine.maxAirJumps > 0 && stateMachine.jumpPhase <= stateMachine.maxAirJumps)
        {
            stateMachine.SwitchState(new PlayerJumpState(stateMachine));
        }
    }
    protected void SetPosition()
    {if (OnGround && !OnSteep)
        {
            //Debug.Log("setting position");
            stateMachine.lastGroundPosition = stateMachine.transform.position;
            stateMachine.lastGroundRotation = stateMachine.transform.rotation;
        }
    }

    protected void SwitchToJumpState()
    {
        jumping = true;
        Debug.Log("switch to jump state move");
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }

    protected void Attack()
    {
        if (!stateMachine.isAttacking)
        {
            Debug.Log("starting attack");
            stateMachine.isAttacking = true;
            stateMachine.animator.speed = stateMachine.baseAnimationSpeed * 1.5f;
            stateMachine.animator.CrossFadeInFixedTime(attackHash, crossFadeDuration);
        }
    }



    protected void CalcVelocity(float acceleration, float speed, Vector3 xAxisIn, Vector3 zAxisIn)
    {
        Vector3 zAxis, xAxis;

        xAxis = xAxisIn;
        zAxis = zAxisIn;

        xAxis = ProjectDirectionOnPlane(xAxis, stateMachine.contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, stateMachine.contactNormal);

        Vector3 relativeVelocity = stateMachine.velocity - stateMachine.connectionVelocity;

        //Debug.Log("relative velocity " + relativeVelocity);

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, stateMachine.upAxis) : 0f;
        //Debug.Log("adjustment before clamp " + adjustment);
        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);
        //Debug.Log("adjustment " + adjustment);
        stateMachine.velocity += xAxis * adjustment.x + zAxis * adjustment.z;

        //Debug.Log("after calc velocity " + stateMachine.velocity);
        if (Swimming)
        {
            stateMachine.velocity += stateMachine.upAxis * adjustment.y;
        }
    }

    public virtual void ContinueAnimation()
    {

    }
}
