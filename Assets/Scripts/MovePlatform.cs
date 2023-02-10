using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public float speed;
    public Vector3 offsetOne;
    public Vector3 offsetTwo;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Vector3 finalPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        targetPos = transform.position + offsetOne;
        finalPos = transform.position + offsetOne + offsetTwo;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        
        if(transform.position == targetPos)
        {
            targetPos = transform.position + offsetTwo;
        }
        if (transform.position == finalPos)
        {
            Destroy(gameObject);
        }
    }
}
