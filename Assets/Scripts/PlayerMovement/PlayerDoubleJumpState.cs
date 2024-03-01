//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerDoubleJumpState : PlayerBaseState
//{
//    private readonly int jumpHash = Animator.StringToHash("Jump");
//    private const float crossFadeDuration = 0.1f;

//    public PlayerDoubleJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

//    public override void Enter()
//    {
//        Debug.Log("enter double jump state");
//        stateMachine.velocity = new Vector3(stateMachine.velocity.x, stateMachine.jumpForce, stateMachine.velocity.z);

//        stateMachine.animator.CrossFadeInFixedTime(jumpHash, crossFadeDuration);
//    }

//    public override void Tick()
//    {
//        ApplyGravity();

//        if (stateMachine.velocity.y <= 0f)
//        {
//            stateMachine.SwitchState(new PlayerFallState(stateMachine));
//        }
//        CalculateMoveDirection();
//        FaceMoveDirection();
//        Move();
//        CheckForClimb();
//    }

//    public override void Exit()
//    {

//    }
//}
