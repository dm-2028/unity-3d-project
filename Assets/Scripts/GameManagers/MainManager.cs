using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    public int beans;
    public string saveFileName;
    public int totalDragonFruit;

    public int health = 10;

    public LevelObject[] levelDataObjects;
    public LevelData[] levelData;

    public int currentLevelIndex;
    public int spawnIndex;

    public Vector3 playerCheckpointPosition { get; private set; }
    public Quaternion playerCheckpointRotation { get; private set; }

    public Vector3 playerLevelPosition { get; private set; }
    public Quaternion playerLevelRotation { get; private set; }

    public static MainManager Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetDefaults();
    }

    private void InitiateLevelData()
    {
        levelData = new LevelData[levelDataObjects.Length];
        for (int i = 0; i < levelDataObjects.Length; i++)
        {
            levelData[i] = new LevelData();
            levelData[i].coffeeBeanCollected = new bool[levelDataObjects[i].totalBeans];
            levelData[i].fruitCollected = new bool[levelDataObjects[i].totalFruit];
            levelData[i].partialFruitCollected = new bool[3];
            levelData[i].cutsceneTriggered = new bool[levelDataObjects[i].totalCutscenes];
            levelData[i].enemyEncounterComplete = new bool[levelDataObjects[i].enemyEncounters];
        }
    }
    private void SetDefaults()
    {
        beans = 0;
        totalDragonFruit = 0;
        health = 10;
        InitiateLevelData();

        currentLevelIndex = 0;

        spawnIndex = 0;
    }

    public void SavePlayerInfo(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        Debug.Log("saving " + data.saveFileName);
        string path = Path.Combine(Application.persistentDataPath, data.saveFileName);
        File.WriteAllText(path, json);
    }
    public void SavePlayerInfo()
    {
        SaveData data = new();

        data.beans = beans;
        data.totalDragonFruit = totalDragonFruit;
        data.saveFileName = saveFileName;
        data.health = health;

        data.levelData = levelData;

        data.currentLevelIndex = currentLevelIndex;

        data.spawnIndex = spawnIndex;

        SavePlayerInfo(data);
    }

    public void SetSpawnIndex(int index)
    {
        spawnIndex = index;
        playerCheckpointPosition = levelDataObjects[currentLevelIndex].spawnPoints[spawnIndex].position;
        playerCheckpointRotation = levelDataObjects[currentLevelIndex].spawnPoints[spawnIndex].rotation;
        SavePlayerInfo();
    }

    public void LoadMostRecent()
    {
        string path = Application.persistentDataPath;

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] allFiles = dirInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
        FileInfo lastModifiedFile = allFiles.OrderBy(fi => fi.LastWriteTime).LastOrDefault();

        LoadPlayerInfo(lastModifiedFile.FullName);

    }

    public FileInfo[] GetAllFiles()
    {
        Debug.Log(Application.persistentDataPath);
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        return dirInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
    }

    public void LoadPlayerInfo(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            beans = data.beans;
            totalDragonFruit = data.totalDragonFruit;
            saveFileName = data.saveFileName;

            currentLevelIndex = data.currentLevelIndex;

            levelData = data.levelData;
            health = data.health;

            spawnIndex = data.spawnIndex;

            playerCheckpointPosition = levelDataObjects[currentLevelIndex].spawnPoints[spawnIndex].position;
            playerCheckpointRotation = levelDataObjects[currentLevelIndex].spawnPoints[spawnIndex].rotation;

            playerLevelPosition = levelDataObjects[currentLevelIndex].spawnPoints[0].position;
            playerLevelRotation = levelDataObjects[currentLevelIndex].spawnPoints[0].rotation;
        }
    }

    public void CreateNewFile(string name){
        SaveData data = new SaveData();
        this.name = data.saveFileName = name;
        SetDefaults();
        SavePlayerInfo(data);
    }
}
