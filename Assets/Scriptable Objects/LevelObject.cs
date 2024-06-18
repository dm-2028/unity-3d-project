using UnityEngine;

[CreateAssetMenu(fileName = "Level Object", menuName = "Level Data")]
public class LevelObject : ScriptableObject
{
    public int levelIndex;
    public int totalBeans;
    public int totalFruit;

    public GameObject[] cutscenes;
    public SpawnPoint[] spawnPoints;
}
[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;
}
