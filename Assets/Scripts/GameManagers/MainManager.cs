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
    public bool[] coffeeBeanCollected;
    public int health = 10;

    public Vector3 playerCheckpointPosition { get; private set; }
    public Quaternion playerCheckpointRotation { get; private set; }

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
        coffeeBeanCollected = new bool[40];
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
        Debug.Log("Collectibles " + coffeeBeanCollected);
        data.coffeeBeanCollected = coffeeBeanCollected;
        data.beans = beans;
        data.saveFileName = saveFileName;
        data.health = health;
        
        data.playerPositionX = playerCheckpointPosition.x;
        data.playerPositionY = playerCheckpointPosition.y;
        data.playerPositionZ = playerCheckpointPosition.z;

        data.playerRotationX = playerCheckpointRotation.x;
        data.playerRotationY = playerCheckpointRotation.y;
        data.playerRotationZ = playerCheckpointRotation.z;
        data.playerRotationW = playerCheckpointRotation.w;

        SavePlayerInfo(data);
    }
    
    public void SaveCheckpoint(Vector3 position, Quaternion rotation)
    {
        Debug.Log("save rotation " + rotation);
        SaveData data = new();
        data.coffeeBeanCollected = coffeeBeanCollected;
        data.beans = beans;
        data.saveFileName = saveFileName;
        data.health = health;
        
        data.playerPositionX = position.x;
        data.playerPositionY = position.y;
        data.playerPositionZ = position.z;

        data.playerRotationX = rotation.x;
        data.playerRotationY = rotation.y;
        data.playerRotationZ = rotation.z;
        data.playerRotationW = rotation.w;

        playerCheckpointPosition = position;
        playerCheckpointRotation = rotation;
        Debug.Log("save rotation " + rotation);

        Debug.Log("save player checkpoint rotation " + data.playerRotationX + " + " + data.playerRotationY + " + " + data.playerRotationZ + " + " + data.playerRotationW);

        Debug.Log("save player checkpoint Rtotation quaternion " + playerCheckpointRotation);
        SavePlayerInfo(data);
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
            saveFileName = data.saveFileName;
            coffeeBeanCollected = data.coffeeBeanCollected;
            health = data.health;

            playerCheckpointPosition = new(data.playerPositionX, data.playerPositionY, data.playerPositionZ);
            playerCheckpointRotation = new Quaternion(data.playerRotationX, data.playerRotationY, data.playerRotationZ, data.playerRotationW);
            Debug.Log("load player checkpoint rotation " + data.playerRotationX + " + " + data.playerRotationY + " + " + data.playerRotationZ);

            Debug.Log("load player checkpoint Rtotation quaternion " + playerCheckpointRotation);
        }
    }
}
