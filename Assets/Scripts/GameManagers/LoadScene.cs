using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneToLoad;

    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
            gameManager.SetCollectibles();
        }
    }
}
