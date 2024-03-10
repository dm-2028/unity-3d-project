using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hoopsObject;
    public GameObject collectibleReward;
    public GameObject challengeSpawner;

    private bool challengeStarted = false;
    private bool challengeAchieved = false;

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<Collectable> collectables = Collectable.FindAll();

        Collectable[] collectablesArray = collectables.ToArray();
        Collectable[] savedCollectables = MainManager.Instance.coffeeBeanList;

        for (int i = 0, j = 0; j < collectablesArray.Length; j++)
        {
            Collectable collectable = collectablesArray[j];
            while (collectable.serializationId != i)
            {
                i++;
            }
            if (i >= savedCollectables.Length)
            {
                break;
            }
            collectable.collected = savedCollectables[i].collected;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (challengeStarted)
        {
            bool challengeComplete = true;
            foreach(Transform child in hoopsObject.transform)
            {
                Debug.Log("check child " + child);
                if (child.gameObject.activeInHierarchy)
                {
                    Debug.Log("set false");
                    challengeComplete = false;
                }
            }
            if (challengeComplete)
            {
                Debug.Log("challenge complete");
                challengeStarted = false;
                challengeAchieved = true;
                collectibleReward.SetActive(true);
            }
        }
    }

    public void StartChallenge(float time)
    {
        
        Debug.Log("start challenge");
        hoopsObject.SetActive(true);
        foreach(Transform child in hoopsObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        challengeStarted = true;
        Invoke("ChallengeReset", time);
    }

    private void ChallengeReset()
    {
        challengeStarted = false;
        if (!challengeAchieved)
        {
            challengeSpawner.SetActive(true);
            hoopsObject.SetActive(false);
        }
    }
}
