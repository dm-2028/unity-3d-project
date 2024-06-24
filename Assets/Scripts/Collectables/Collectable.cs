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

    virtual protected void Awake()
    {
        UpdateVisibility();
    }

    protected void Loaded()
    {
        UpdateVisibility();
    }

    virtual public void SetCollected(bool collected)
    {
        this.collected = collected;
        UpdateVisibility();
    }

    virtual public void UpdateVisibility()
    {
        Debug.Log("Update Visibility " + Tag + " " + collected);
        switch (Tag)
        {
            case CollectableType.CoffeeBean:
            case CollectableType.PartialDragonFruit:
            case CollectableType.EnemyEncounter:
                transform.parent.gameObject.SetActive(!collected);
                break;
            case CollectableType.Cutscene:
                gameObject.SetActive(!collected);
                break;
        }
    }
}

public static class CollectableType
{
    public const string CoffeeBean = "CoffeeBean";
    public const string PartialDragonFruit = "PartialDragonFruit";
    public const string Cutscene = "Cutscene";
    public const string EnemyEncounter = "EnemyEncounter";
}