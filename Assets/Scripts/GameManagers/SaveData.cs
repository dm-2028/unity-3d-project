[System.Serializable]
public class SaveData
{
    public string saveFileName;
    public int beans;

    public LevelData[] levelData;

    public int health;

    public int currentLevelIndex;

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float playerRotationX;
    public float playerRotationY;
    public float playerRotationZ;
    public float playerRotationW;

    public float levelPositionX;
    public float levelPositionY;
    public float levelPositionZ;

    public float levelRotationX;
    public float levelRotationY;
    public float levelRotationZ;
    public float levelRotationW;
}