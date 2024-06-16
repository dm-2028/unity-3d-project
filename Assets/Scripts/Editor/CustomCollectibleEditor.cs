using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Collectable), true)]
[CanEditMultipleObjects]
public class CustomCollectibleEditor : Editor
{
    private SerializedProperty _serializationId;

    private static int FindNextId()
    {
        var collectables = Collectable.FindAll(CoffeeBean.Tag);
        var last = collectables.LastOrDefault();
        var nextId = last == null ? 0 : last.serializationId + 1;

        var exists = new HashSet<int>(collectables.Select(o => o.serializationId));
        for(int i = 0; i < nextId; i++)
        {
            if (!exists.Contains(i))
                return i;
        }
        return nextId;
    }

    public override void OnInspectorGUI()
    {
        var collectable = (Collectable)target;
        var isPrefab = PrefabUtility.GetCorrespondingObjectFromSource(collectable.gameObject) == null;
        LayerMask layerMask = -1;
        //if(Physics.Raycast(collectable.transform.position, Vector3.down, out RaycastHit hit, 2f, layerMask, QueryTriggerInteraction.Ignore)){
        //    collectable.transform.position = new(collectable.transform.position.x, hit.point.y + .5f, collectable.transform.position.z);
        //}
        if(!isPrefab && collectable.serializationId == -1)
        {
            collectable.serializationId = FindNextId();
            _serializationId.intValue = collectable.serializationId;
            EditorUtility.SetDirty(target);
        }

        DrawDefaultInspector();
    }

    private void Awake()
    {
         _serializationId = serializedObject.FindProperty("serializationId");
    }
    
}
