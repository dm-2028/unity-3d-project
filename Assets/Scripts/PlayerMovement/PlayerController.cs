//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerInput))]
//[RequireComponent(typeof(CharacterController))]
//public class PlayerController : MonoBehaviour
//{

//    public float horizontalInput;
//    public float verticalInput;

//    public float speed = 10.0f;
//    public float gravityValue = 9.81f;
//    public float jumpHeight = 2.0f;
//    private Vector3 playerVelocity;

//    private bool grounded;
//    public int maxJumps = 1;

//    private Vector2 movement;

//    public Transform leftArm;
//    public Transform rightArm;

//    private PlayerInput playerInput;
//    private PlayerControls playerControls;
//    private CharacterController controller;


//    private void Awake()
//    {
//        controller = GetComponent<CharacterController>();
//        playerControls = new PlayerControls();
//        playerInput = GetComponent<PlayerInput>();
//    }

//    private void OnEnable()
//    {
//        playerControls.Enable();
//    }

//    private void OnDisable()
//    {
//        playerControls.Disable();
//    }
//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        HandleInput();
//        HandleMovement();
//        HandleRotation();
//        grounded = controller.isGrounded;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {

//    }

//    void HandleInput()
//    {
//        movement = playerControls.Controls.Movement.ReadValue<Vector2>();
        
//    }

//    void HandleMovement()
//    {
//        Vector3 move = new Vector3(-movement.x, 0, -movement.y);
//        controller.Move(move * Time.deltaTime * speed);

//        if(move != Vector3.zero)
//        {
//            gameObject.transform.forward = move;
//        }

//        playerControls.Controls.Jump.performed += Jump_performed;

//        //playerVelocity.y += gravityValue * Time.deltaTime;
//        controller.Move(playerVelocity * Time.deltaTime);
//    }

//    private void Jump_performed(InputAction.CallbackContext obj)
//    {
//        if (grounded)
//        {
//            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

//        }
//    }
//    void HandleRotation()
//    {

//    }

//    void OnJump(InputValue value)
//    {
//        Debug.Log("logging jump");
//    }

//    void onMovement(InputValue value)
//    {

//    }

//    void onCamera(InputValue value)
//    {

//    }
//}

