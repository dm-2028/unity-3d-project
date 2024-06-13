using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().SetCollectibles();
    }
}
