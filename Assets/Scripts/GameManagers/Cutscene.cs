using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : Collectable
{
    override public string Tag
    {
        get
        {
            return CollectableType.Cutscene;
        }
    }

    [SerializeField]
    GameObject NPC;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStateMachine>().StartTalking(NPC);
            collected = true;

            MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].cutsceneTriggered[serializationId] = true;
            gameObject.SetActive(false);
        }
    }
}
