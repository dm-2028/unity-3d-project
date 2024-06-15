using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Collectable : MonoBehaviour
{
    public static IEnumerable<Collectable> FindAll(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        return objects
            .Select(o => o.transform.GetComponent<Collectable>())
            .Where(o => o != null)
            .OrderBy(o => o.serializationId);
    }

    //[HideInInspector]
    public int serializationId = -1;

    public bool collected = false;
}
