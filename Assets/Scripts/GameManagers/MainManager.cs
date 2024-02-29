using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    int shipSelection, highScore;

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

        LoadPlayerInfo();
    }

    [System.Serializable]
    class SaveData
    {
        public int selection;
        public int highScore;
    }

    public void SavePlayerInfo()
    {
        SaveData data = new SaveData();

        data.selection = shipSelection;
        data.highScore = highScore;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
    }

    public void LoadPlayerInfo()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            highScore = data.highScore;
            shipSelection = data.selection;
        }
    }
}
