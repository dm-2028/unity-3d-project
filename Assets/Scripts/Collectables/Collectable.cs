using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

    public const string Tag  = "Collectable";

    public static IEnumerable<Collectable> FindAll()
    {
        return GameObject.FindGameObjectsWithTag(Tag)
            .Select(o => o.GetComponent<Collectable>())
            .Where(o => o != null)
            .OrderBy(o => o.serializationId);
    }

    //[HideInInspector]
    public int serializationId = -1;

    public bool collected = false;
}
