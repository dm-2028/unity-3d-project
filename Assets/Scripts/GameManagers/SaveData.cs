using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string saveFileName;
    public int beans;
    public bool[] coffeeBeanCollected;
    public bool[][] partialFruitCollected;
    public bool[] fruitCollected;

    public bool[] cutsceneTriggered;
    public int health;

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