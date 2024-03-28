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

    private bool sceneShown = false;
    private bool activeSpawning = false;
    private int enemiesActive = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;

    // Start is called before the first frame update
    void Start()
    {
        activeEnemies = new List<GameObject>(spawnLimit);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!sceneShown) {
                PlayerStateMachine player = GetComponent<PlayerStateMachine>();
                if (player.groundContactCount > 0)
                {
                    CameraStart();
                }
            } 
            else if (activeSpawning)
            {
                while(enemiesActive < spawnLimit && enemiesSpawned < enemyCount)
                {
                    int spawnIndex = Random.Range(0, spawnPositions.Length);
                    GameObject enemy = Instantiate(enemyPrefab, spawnPositions[spawnIndex], spawnRotations[spawnIndex]);
                    enemy.GetComponent<EnemyStateMachine>().enemySpawn = this;
                    activeEnemies.Add(enemy);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(GameObject enemy in activeEnemies)
            {
                EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
                enemyStateMachine.SwitchState(new EnemyMoveState(enemyStateMachine));
            }
        }
    }

    public void incrementKilled(GameObject enemy)
    {
        enemiesKilled++;
        activeEnemies.Remove(enemy);
        if(enemiesKilled == enemyCount)
        {
            activeSpawning = false;
        }
    }

    IEnumerator CameraStart()
    {
        yield return null;
        sceneShown = true;
        activeSpawning = true;
    }

    IEnumerator CameraFinish()
    {
        yield return null;
    }
}
