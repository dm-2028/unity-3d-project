using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject hoopsObject;
    public GameObject collectibleReward;
    public GameObject challengeSpawner;

    public GameObject pauseMenu;

    public TextMeshProUGUI beansText;

    public Slider healthBar;

    private bool gamePaused = false;
    private bool challengeStarted = false;
    private bool challengeAchieved = false;

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<Collectable> collectables = Collectable.FindAll();

        Collectable[] collectablesArray = collectables.ToArray();
        if (MainManager.Instance.coffeeBeanCollected == null || MainManager.Instance.coffeeBeanCollected.Length == 0)
        {
            MainManager.Instance.coffeeBeanCollected = new bool[collectablesArray.Length];
        }
        bool[] collected = MainManager.Instance.coffeeBeanCollected;

        for (int i = 0, j = 0; j < collectablesArray.Length; j++)
        {
            Collectable collectable = collectablesArray[j];
            while (collectable.serializationId != i)
            {
                i++;
            }
            if (i >= collected.Length)
            {
                break;
            }
            collectable.collected = collected[i];
            collectable.gameObject.transform.GetChild(0).gameObject.SetActive(!collectable.collected);
        }
        Debug.Log("finished setting collectables ");
        pauseMenu.gameObject.SetActive(false);
        beansText.text = "Beans: " + MainManager.Instance.beans.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        if (challengeStarted)
        {
            bool challengeComplete = true;
            foreach(Transform child in hoopsObject.transform)
            {
                Debug.Log("check child " + child);
                if (child.gameObject.activeInHierarchy)
                {
                    Debug.Log("set false");
                    challengeComplete = false;
                }
            }
            if (challengeComplete)
            {
                Debug.Log("challenge complete");
                challengeStarted = false;
                challengeAchieved = true;
                collectibleReward.SetActive(true);
            }
        }
    }

    public void StartChallenge(float time)
    {
        
        Debug.Log("start challenge");
        hoopsObject.SetActive(true);
        foreach(Transform child in hoopsObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        challengeStarted = true;
        Invoke("ChallengeReset", time);
    }

    private void ChallengeReset()
    {
        challengeStarted = false;
        if (!challengeAchieved)
        {
            challengeSpawner.SetActive(true);
            hoopsObject.SetActive(false);
        }
    }

    public static void SaveData()
    {
        //IEnumerable<Collectable> collectables = Collectable.FindAll();

        //Collectable[] collectablesArray = collectables.ToArray();
        //for(int i = 0; i < collectablesArray.Length; i++)
        //{
        //    Debug.Log("collectable " + collectablesArray[i].serializationId + " " + collectablesArray[i].collected);
        //}
        //Debug.Log("lenth " + collectablesArray.Length.ToString());
        //bool[] asArray = new bool[collectablesArray.Length];
        //foreach(Collectable collectable in collectablesArray)
        //{
        //    Debug.Log("array length " + asArray.Length.ToString());
        //    Debug.Log("serialization " + collectable.serializationId.ToString());
        //    asArray[collectable.serializationId] = collectable.collected;
        //}
        //MainManager.Instance.coffeeBeanCollected = asArray;
        MainManager.Instance.SavePlayerInfo();

    }

    private void TogglePause()
    {
        if (!gamePaused)
        {
            gamePaused = true;
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            gamePaused = false;
            pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void UpdateHealth(int health)
    {
        healthBar.value = health;
        //MainManager.Instance.health = health;
        //MainManager.Instance.SavePlayerInfo();
    }

    public void IncrementBeans()
    {
        MainManager.Instance.beans++;
        beansText.text = "Beans: " + MainManager.Instance.beans.ToString();
        MainManager.Instance.SavePlayerInfo();
    }
}
