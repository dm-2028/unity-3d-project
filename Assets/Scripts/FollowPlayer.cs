using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private float radius = 5.0f;
    public float horizontalInput;
    private float speed = 10.0f;

    private PlayerInput playerInput;
    private PlayerControls playerControls;

    private Vector2 playerCamera;
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        horizontalInput = Input.GetAxis("CameraHorizontal");
        transform.position = player.transform.position - (transform.forward*radius);
        transform.RotateAround(player.transform.position, player.transform.up, horizontalInput * 20 * Time.deltaTime * speed);
        
    }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
    }

    void HandleInput()
    {
        playerCamera = playerControls.Controls.Camera.ReadValue<Vector2>();
    }

    void HandleMovement()
    {
        Vector3 move = new Vector3(playerCamera.x, 0, playerCamera.y);

    }
}
