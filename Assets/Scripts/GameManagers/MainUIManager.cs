using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public void ExitGame()
    {
        Time.timeScale = 1;
        MainManager.Instance.SavePlayerInfo();
        SceneManager.LoadScene(0);
    }

    public void Close()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

}