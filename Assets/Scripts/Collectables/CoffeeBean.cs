using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CoffeeBean : Collectable
{
    // Start is called before the first frame update
    void Start()
    {
        
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

    private void Awake()
    {
        UpdateVisibility();
        Debug.Log("loading " + serializationId + " " + collected);
    }

    void Loaded()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        Debug.Log("updating visibility in " + serializationId);
        if (collected)
        {
            transform.GetChild(0).gameObject.SetActive(false);

        }
        else transform.GetChild(0).gameObject.SetActive(true);
    }
}
