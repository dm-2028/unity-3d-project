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
        Debug.Log("loading " + serializationId + " " + collected);
        var newRotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 60.0f, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime);
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
        if (collected)
        {
            gameObject.SetActive(false);

        }
        else gameObject.SetActive(true);
        
    }
}
