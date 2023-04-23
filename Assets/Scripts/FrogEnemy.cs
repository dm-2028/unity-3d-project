using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrogEnemy : MonoBehaviour
{

    [SerializeField] bool isGrounded;
    [SerializeField] bool isJumping;

    private Rigidbody playerRb;

    public MeshRenderer boundary;

    [Range(0.1f, 1f)] public float sphereCastRadius = .1f;
    [Range(0f, 100f)] public float range = .1f;
    public LayerMask layerMask;

    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        isJumping = false;
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (isGrounded && !isJumping)
        {
            StartCoroutine("Jump");

        }
    }

    Vector3 calculateJump()
    {
        return new Vector3(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f), 0) * Random.Range(3f, 5f);
    }

    IEnumerator Jump()
    {
        isJumping = true;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = transform.rotation * Quaternion.Euler(new Vector3(0, Random.Range(10, 90)* (Random.Range(0, 2) * 2 - 1), 0));
        float i = 0.0f;
        float rate = 1.0f / Random.Range(1.0f, 3.0f);
        while(i < 1.0)
        {
            i += Time.deltaTime * rate;
            transform.rotation = Quaternion.Lerp(startRot, endRot, i);
            yield return null;
        }
        isGrounded = false;
        playerRb.AddRelativeForce(calculateJump(), ForceMode.Impulse);
        isJumping = false;
    }
    void GroundCheck()
    {
        Vector3 origin = transform.position;
        Vector3 direction = -transform.up;
        Debug.DrawRay(origin, direction, Color.white);
        if (Physics.SphereCast(transform.position, sphereCastRadius, -transform.up * range, out RaycastHit hit, range, layerMask))
        {
            Debug.Log("ground check true");
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, sphereCastRadius, -transform.up * range, out hit, range, layerMask))
        {
            Gizmos.color = Color.green;
            Vector3 sphereCastMidpoint = transform.position + (-transform.up * hit.distance);
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = transform.position + (-transform.up * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
        }
    }
}
