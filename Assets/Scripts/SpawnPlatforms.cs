using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatforms : MonoBehaviour
{
    public GameObject platformPrefab;
    public Vector3 position;
    public float spawnFrequency;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPlatform", 0f, spawnFrequency);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlatform()
    {
        Instantiate(platformPrefab, position, platformPrefab.transform.rotation);
    }
}
