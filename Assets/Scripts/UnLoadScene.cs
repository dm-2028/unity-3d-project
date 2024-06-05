using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnLoadScene : MonoBehaviour
{
    public string sceneToUnLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.GetSceneByName(sceneToUnLoad).isLoaded){ 
                SceneManager.UnloadSceneAsync(sceneToUnLoad); 
            }
        }
    }
}
