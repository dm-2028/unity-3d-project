using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWheel : MonoBehaviour
{
    public float rotationSpeed = 15.0f;

    [SerializeField]
    Vector3 axis = Vector3.forward;

    [SerializeField]
    bool reverse = false;
    [SerializeField]
    bool backAndForth = false;
    [SerializeField]
    float maxAngle = 180f;
    [SerializeField]
    float factor = 5f;

    bool reversing = false;

    Quaternion startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        startingRotation = transform.rotation ;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Quaternion.Angle(transform.rotation, startingRotation));
        if (backAndForth && Quaternion.Angle(transform.rotation, startingRotation) > maxAngle)
        {
            reversing = !reversing;
        }
        transform.Rotate(axis, rotationSpeed * Time.deltaTime * (reverse ? -1 : 1) * (reversing ? 1 : -1));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {

            EvaluateCollision(collision);

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            EvaluateCollision(collision);

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            var player = collision.gameObject.GetComponent<PlayerStateMachine>();
            player.body.velocity = new Vector3(0f, 0f, 0f);
            player.velocity = player.body.velocity;
        }
    }

    void EvaluateCollision(Collision collision)
    {
        Debug.Log("evaluating collision");
        var player = collision.gameObject.GetComponent<PlayerStateMachine>();


        
        if (player.submergence >= player.swimThreshold)
        {
            Vector3 direction = collision.transform.position - transform.position;
            Debug.Log("direction before " + direction);
            direction = transform.InverseTransformDirection(direction);

            Vector3 xDirection = new Vector3(direction.x, 0f, 0f);

            xDirection = transform.TransformDirection(xDirection);
            xDirection.Normalize();

            player.body.velocity += xDirection*factor;
            player.velocity = player.body.velocity;
        }
    }
}
