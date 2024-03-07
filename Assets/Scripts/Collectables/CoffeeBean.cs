using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoffeeBean : Collectable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var newRotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 30.0f, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);
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
            this.enabled = false;
        else this.enabled = true;
    }
}
