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
}
