using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrogEnemy : MonoBehaviour
{

    [SerializeField] bool isGrounded;

    private Rigidbody playerRb;

    public MeshRenderer boundary;


    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (isGrounded)
        {
            isGrounded = false;
            playerRb.AddForce(calculateJump(), ForceMode.Impulse);
        }
    }

    Vector3 calculateJump()
    {
        return new Vector3(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f), 0) * Random.Range(3f, 5f);
    }

    void GroundCheck()
    {
        Vector3 origin = transform.position;
        Vector3 direction = -transform.up;
        Debug.DrawRay(origin, direction, Color.white);
        if (Physics.SphereCast(origin, .1f, direction, out RaycastHit hit, .1f))
        {
            isGrounded = true;
        }
    }
}
