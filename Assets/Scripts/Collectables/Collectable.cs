using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Collectable : MonoBehaviour
{
    abstract public string Tag { get; }
    public static IEnumerable<Collectable> FindAll(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Collectable");

        return objects
            .Select(o => o.transform.GetComponentInChildren<Collectable>())
            .Where(o => o != null)
            .Where(o => o.Tag == tag)
            .OrderBy(o => o.serializationId);
    }

    //[HideInInspector]
    public int serializationId = -1;

    public int levelId = -1;

    public bool collected = false;
}

public static class CollectableType
{
    public const string CoffeeBean = "CoffeeBean";
    public const string PartialDragonFruit = "PartialDragonFruit";
}