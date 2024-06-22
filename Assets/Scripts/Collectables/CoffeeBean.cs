using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeBean : Collectable
{

    override public string Tag
    {
        get
        {
            return CollectableType.CoffeeBean;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!collected)
        {
            var newRotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 60.0f, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);
        }
    }
}
