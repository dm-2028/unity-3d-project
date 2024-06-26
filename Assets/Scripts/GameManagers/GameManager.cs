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
    public TextMeshProUGUI dragonFruitText;

    public Slider healthBar;

    private bool challengeStarted = false;
    private bool challengeAchieved = false;

    public GameObject playerPrefab;

    public MainUIManager menu;

    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Intro Level", LoadSceneMode.Additive);
        pauseMenu.gameObject.SetActive(false);
        menu = GameObject.FindGameObjectWithTag("Menu").GetComponent<MainUIManager>();
        if(MainManager.Instance.playerCheckpointPosition == Vector3.zero || MainManager.Instance.playerCheckpointRotation == new Quaternion(0, 0, 0, 0))
        {
            MainManager.Instance.SetSpawnIndex(0);
        }
        foreach(int scene in MainManager.Instance.levelDataObjects[MainManager.Instance.currentLevelIndex].spawnPoints[MainManager.Instance.spawnIndex].sectionToLoad)
        {
            SceneManager.LoadScene("Intro Level Section " + scene.ToString(), LoadSceneMode.Additive);
        }

        _player = Instantiate(playerPrefab, MainManager.Instance.playerCheckpointPosition, MainManager.Instance.playerCheckpointRotation);

        GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.GetComponent<OrbitCamera>().focus = _player.transform;
        mainCamera.GetComponent<OrbitCamera>().SetAngle(30f, _player.transform.rotation.eulerAngles.y-180);

        menu.UpdateCollectableText();


    }

    public void SetCollectibles()
    {
        _SetCollectibles(CollectableType.CoffeeBean, MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].coffeeBeanCollected);
        _SetCollectibles(CollectableType.PartialDragonFruit, MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].partialFruitCollected);
        _SetCollectibles(CollectableType.Cutscene, MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].cutsceneTriggered);
        _SetCollectibles(CollectableType.EnemyEncounter, MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].enemyEncounterComplete);
        Debug.Log("finished setting collectables ");
    }

    private void _SetCollectibles(string tag, bool[] _collectables)
    {
        IEnumerable<Collectable> collectables = Collectable.FindAll(tag);
        Debug.Log(" collectables " + collectables.ToArray().Length.ToString() + " " + collectables.ToArray().ToString());

        Collectable[] collectablesArray = collectables.ToArray();

        bool[] collected = _collectables;

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
            collectable.SetCollected(collected[i]);
        }
    }
    // Update is called once per frame
    void Update()
    {

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

  


    public void UpdateHealth(int health)
    {
        healthBar.value = health;
        MainManager.Instance.health = health;
        if(health <= 0)
        {
            Destroy(_player);
            GameObject.FindGameObjectWithTag("Menu").GetComponent<MainUIManager>().GameOver();
        }
    }

    public void IncrementBeans()
    {
        MainManager.Instance.beans++;
        menu.UpdateCollectableText();
        MainManager.Instance.SavePlayerInfo();
    }

    public void CollectPartialFruit()
    {
        if(MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].partialFruitCollected.All(o => o))
        {
            MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].fruitCollected[0] = true;
            MainManager.Instance.totalDragonFruit++;

            //TODO: remove this
            GameObject.FindGameObjectWithTag("Menu").GetComponent<MainUIManager>().GameOver();
        }
        menu.UpdateCollectableText();
        MainManager.Instance.SavePlayerInfo();
    }
}
