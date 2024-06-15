using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartialDragonFruit : Collectable
{

    static public string Tag = "PartialDragonFruit";

    [SerializeField]
    [Range(0,2)]
    int pieceNumber;

    float yPos;

    [SerializeField]
    float floatHeight, speed;

    private void Start()
    {
        yPos = transform.position.y;
    }
    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            var newRotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 60.0f, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);

            var pos = transform.position;
            var newY = yPos + floatHeight * Mathf.Sin(Time.time * speed);
            transform.position = new Vector3(pos.x, newY, pos.z);
        }
    }

    private void Awake()
    {
        UpdateVisibility();
    }

    void Loaded()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (collected)
        {
            gameObject.SetActive(false);

        }
        else gameObject.SetActive(true);
    }
}

