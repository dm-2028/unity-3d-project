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

    protected void Awake()
    {
        UpdateVisibility();
    }

    protected void Loaded()
    {
        UpdateVisibility();
    }

    protected void UpdateVisibility()
    {
        if (collected)
        {
            gameObject.transform.parent.gameObject.SetActive(false);

        }
        else gameObject.transform.parent.gameObject.SetActive(true);
    }
}

public static class CollectableType
{
    public const string CoffeeBean = "CoffeeBean";
    public const string PartialDragonFruit = "PartialDragonFruit";
    public const string Cutscene = "Cutscene";
    public const string EnemyEncounter = "EnemyEncounter";
}