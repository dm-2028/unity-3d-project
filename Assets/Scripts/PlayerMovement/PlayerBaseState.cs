using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    // Start is called before the first frame update

    public float offsetFromWall = 0.3f;
    protected Transform helper = new GameObject().transform;
    protected LayerMask ignoreLayers;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        helper.name = "Climb Helper";
        this.stateMachine = stateMachine;

    }

    protected void CalculateMoveDirection()
    {
        Vector3 cameraForward = new(stateMachine.mainCamera.forward.x, 0, stateMachine.mainCamera.forward.z);
        Vector3 cameraRight = new(stateMachine.mainCamera.right.x, 0, stateMachine.mainCamera.right.z);

        Vector3 moveDirection = cameraForward.normalized * stateMachine.inputReader.movement.y + cameraRight.normalized * stateMachine.inputReader.movement.x;

        stateMachine.velocity.x = moveDirection.x * stateMachine.movementSpeed;
        stateMachine.velocity.z = moveDirection.z * stateMachine.movementSpeed;
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

    protected void CheckForClimb()
    {
        Debug.Log("Check for Climb");
        Vector3 origin = stateMachine.transform.position;
        origin.y += 1f;
        Vector3 dir = stateMachine.transform.forward;
        Debug.Log("origin location " + origin);
        Debug.Log("direction location " + dir);
        Debug.Log("ignore " + ignoreLayers.value);
        Debug.DrawRay(origin, dir,Color.white);
        if (Physics.Raycast(origin, dir, out RaycastHit hit, 1f, ignoreLayers))
        {
            Debug.Log("climb state?");
            helper.position = PosWithOffset(origin, hit.point);
            stateMachine.SwitchState(new PlayerClimbState(stateMachine, hit));
        }

    }

    protected Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }
}
