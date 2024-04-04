using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Vector3[] spawnPositions;
    public Quaternion[] spawnRotations;
    public GameObject enemyPrefab;
    public int spawnLimit = 2;
    public int enemyCount = 4;
    public List<GameObject> activeEnemies;

    public GameObject animateObject;
    private Animation mAnimation;

    private bool sceneShown = false;
    private bool activeSpawning = false;
    private bool playerInTrigger = false;
    private int enemiesActive = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private int spawnIndex;

    // Start is called before the first frame update
    void Start()
    {
        mAnimation = animateObject.GetComponent<Animation>();
        activeEnemies = new List<GameObject>(spawnLimit);
        int spawnIndex = Random.Range(0, spawnPositions.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger)
        {

            if (activeSpawning)
            {
                while (enemiesActive < spawnLimit && enemiesSpawned < enemyCount)
                {

                    GameObject enemy = Instantiate(enemyPrefab, spawnPositions[spawnIndex], spawnRotations[spawnIndex]);
                    EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
                    enemyStateMachine.enemySpawn = this;
                    activeEnemies.Add(enemy);
                    
                    enemiesActive++;
                    enemiesSpawned++;
                    spawnIndex = (spawnIndex + 1) % spawnPositions.Length;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        Debug.Log("set in trigger true");

        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (!sceneShown && other.CompareTag("Player"))
        {

            PlayerStateMachine player = other.gameObject.GetComponent<PlayerStateMachine>();
            Debug.Log(player.groundContactCount + " is the contact count");
            //if (player.groundContactCount > 0)
            //{
                Debug.Log("camera start");

                StartCoroutine(CameraStart());
            //}
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("collider exit");

        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            foreach(GameObject enemy in activeEnemies)
            {
                EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
                enemyStateMachine.SwitchState(new EnemyWanderState(enemyStateMachine));
            }
        }
    }

    public void incrementKilled(GameObject enemy)
    {
        enemiesKilled++;
        activeEnemies.Remove(enemy);
        enemiesActive--;
        if(enemiesKilled == enemyCount)
        {
            activeSpawning = false;
            mAnimation.Play();
            StartCoroutine(CameraFinish());
        }
    }

    IEnumerator CameraStart()
    {
        Debug.Log("camera starting");
        while (false)
        {
            yield return null;
        }
        Debug.Log("setting scenshown");
        sceneShown = true;
        activeSpawning = true;
    }

    IEnumerator CameraFinish()
    {
        while (false)
        {
            yield return null;
        }
    }
}
