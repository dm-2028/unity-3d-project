using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Collectable : MonoBehaviour
{

    public const string Tag  = "Collectable";

    public static IEnumerable<Collectable> FindAll()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(Tag);
        for(int i = 0; i < objects.Length; i++)
        {
            Debug.Log(i + " " + objects[i].transform.GetComponent<Collectable>().serializationId);
             
        }

        return objects
            .Select(o => o.transform.GetComponent<Collectable>())
            .Where(o => o != null)
            .OrderBy(o => o.serializationId);
    }

    //[HideInInspector]
    public int serializationId = -1;

    public bool collected = false;
}
