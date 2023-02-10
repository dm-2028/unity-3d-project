using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChallenge : MonoBehaviour
{
    public GameManager gameManager;
    public float challengeTime = 30.0f;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("player enter");
        if (other.CompareTag("Player"))
        {
            gameManager.StartChallenge(challengeTime);
            gameObject.SetActive(false);
        }
    }
}
