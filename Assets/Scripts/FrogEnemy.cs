using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FrogEnemy : MonoBehaviour
{

    [SerializeField] bool isGrounded;
    [SerializeField] bool isJumping;
    [SerializeField] bool isOutsideBoundary;

    private Rigidbody playerRb;

    public MeshRenderer boundary;

    public float sphereCastRadius = .1f;
    public float range = .45f;
    public LayerMask layerMask;

    Quaternion targetRot;

    GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        isOutsideBoundary = false;
        isJumping = false;
        layerMask = (1 << 0);
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        if (isGrounded && !isJumping)
        {
            Debug.Log("starting jump coroutine");
            StartCoroutine("Jump");

        }
    }

    Vector3 calculateJump()
    {
        return new Vector3(Random.Range(.8f, 1.2f), Random.Range(.8f, 1.2f), 0) * Random.Range(3f, 5f);
    }

    IEnumerator Jump()
    {
        Debug.Log("start jump coroutine");
        isJumping = true;
        Quaternion startRot = transform.rotation;
        if (!isOutsideBoundary)
        {
            targetRot = transform.rotation * Quaternion.Euler(new Vector3(0, Random.Range(10, 90) * (Random.Range(0, 2) * 2 - 1), 0));
        }
        float i = 0.0f;
        float rate = 1.0f / Random.Range(1.0f, 3.0f);
        while(i < 1.0)
        {
            i += Time.deltaTime * rate;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, i);
            yield return null;
        }
        Debug.Log("start jumping");
        playerRb.AddRelativeForce(calculateJump(), ForceMode.Impulse);
        Debug.Log("is grounded false");
        isGrounded = false;
        isJumping = false;
    }
    void GroundCheck()
    {
        Vector3 origin1 = transform.position + new Vector3(-1.25f, 0, .75f);
        Vector3 origin2 = transform.position + new Vector3(-1.25f, 0, -.75f);
        Vector3 origin3 = transform.position + new Vector3(.5f, 0, .75f);
        Vector3 origin4 = transform.position + new Vector3(.5f, 0, -75f);
        Vector3 direction = -transform.up;
        if ((Physics.SphereCast(origin1, sphereCastRadius, direction * range, out RaycastHit hit1, range, layerMask) ||
            Physics.SphereCast(origin2, sphereCastRadius, direction * range, out RaycastHit hit2, range, layerMask) ||
            Physics.SphereCast(origin3, sphereCastRadius, direction * range, out RaycastHit hit3, range, layerMask) ||
            Physics.SphereCast(origin4, sphereCastRadius, direction * range, out RaycastHit hit4, range, layerMask)) && 
            !isJumping)
        {
            Debug.Log("is grounded true");
            isGrounded = true;
        }
        else
        {
            Debug.Log("is grounded false");
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
            Vector3 sphereCastMidpoint1 = transform.position + (-transform.up * hit.distance) + new Vector3(-1.25f, 0, .75f);
            Gizmos.DrawWireSphere(sphereCastMidpoint1, sphereCastRadius);
            Vector3 sphereCastMidpoint2 = transform.position + (-transform.up * hit.distance) + new Vector3(-1.25f, 0, -.75f);
            Gizmos.DrawWireSphere(sphereCastMidpoint2, sphereCastRadius);
            Vector3 sphereCastMidpoint3 = transform.position + (-transform.up * hit.distance) + new Vector3(.5f, 0, .75f); ;
            Gizmos.DrawWireSphere(sphereCastMidpoint3, sphereCastRadius);
            Vector3 sphereCastMidpoint4 = transform.position + (-transform.up * hit.distance) + new Vector3(.5f, 0, -75f);
            Gizmos.DrawWireSphere(sphereCastMidpoint4, sphereCastRadius);

            Gizmos.DrawSphere(hit.point, 0.1f);
            Debug.DrawLine(transform.position, sphereCastMidpoint1, Color.green);
            Debug.DrawLine(transform.position, sphereCastMidpoint1, Color.green);
            Debug.DrawLine(transform.position, sphereCastMidpoint1, Color.green);
            Debug.DrawLine(transform.position, sphereCastMidpoint1, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = transform.position + (-transform.up * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("FrogBoundary"))
        {
            Debug.Log("on trigger exit");
            isOutsideBoundary = true;
            targetRot = transform.rotation * Quaternion.FromToRotation(transform.position, other.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("FrogBoundary"))
        {
            Debug.Log("on trigger enter");
            isOutsideBoundary = false;
        }
    }
}
