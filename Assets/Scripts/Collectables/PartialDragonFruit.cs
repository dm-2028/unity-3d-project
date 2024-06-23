using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartialDragonFruit : Collectable
{

    override public string Tag
    {
        get
        {
            return CollectableType.PartialDragonFruit;
        }
    }


    float yPos;

    [SerializeField]
    float floatHeight, speed;
    public bool beingPulled { get; set; } = false;

    private void Start()
    {
        yPos = transform.localPosition.y;
    }
    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            var newRotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 60.0f, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);
            if (!beingPulled)
            {
                var pos = transform.localPosition;
                var newY = yPos + floatHeight * Mathf.Sin(Time.time * speed);
                transform.localPosition = new Vector3(pos.x, newY, pos.z);
            }
        }
    }


}

