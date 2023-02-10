using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject hoopsObject;
    public GameObject collectibleReward;
    public GameObject challengeSpawner;

    private bool challengeStarted = false;
    private bool challengeAchieved = false;

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
