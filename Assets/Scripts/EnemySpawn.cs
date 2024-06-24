using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : Collectable
{

    override public string Tag
    {
        get
        {
            return CollectableType.EnemyEncounter;
        }
    }
    public Vector3[] spawnPositions;
    public Quaternion[] spawnRotations;
    public GameObject enemyPrefab;
    public int spawnLimit = 2;
    public int enemyCount = 4;
    public List<GameObject> activeEnemies;
    private GameObject player;
    public GameObject reward;
    public Vector3 rewardSetPosition;

    public GameObject animateObject;
    private Animation mAnimation;

    private bool sceneShown = false;
    private bool activeSpawning = false;
    private bool playerInTrigger = false;
    private int enemiesActive = 0;
    private int enemiesSpawned = 0;
    private int enemiesKilled = 0;
    private int spawnIndex;

    private OrbitCamera orbitCamera;

    override protected void Awake()
    {
        base.Awake();
        mAnimation = animateObject.GetComponent<Animation>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        activeEnemies = new List<GameObject>(spawnLimit);
        int spawnIndex = Random.Range(0, spawnPositions.Length);
        orbitCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();
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
                    //GameObject worm = Instantiate(wormPrefab, spawnPositions[spawnIndex] + new Vector3(-12,0,1), spawnRotations[spawnIndex]);
                    GameObject enemy = Instantiate(enemyPrefab, spawnPositions[spawnIndex], spawnRotations[spawnIndex]);
                    RatStateMachine enemyStateMachine = enemy.GetComponent<RatStateMachine>();
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
                RatStateMachine enemyStateMachine = enemy.GetComponent<RatStateMachine>();
                enemyStateMachine.SwitchState(new RatWanderState(enemyStateMachine));
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
            StartCoroutine(CameraFinish());
        }
    }

    IEnumerator CameraStart()
    {
        Debug.Log("camera starting");
        player.GetComponent<PlayerStateMachine>().inputReader.controls.Controls.Disable();
        orbitCamera.SetFocusPoint(spawnPositions[0], new Vector2(30, 140));
        sceneShown = true;
        activeSpawning = true;
        yield return new WaitForSeconds(2);
        orbitCamera.ResetFocusToPlayer();
        player.GetComponent<PlayerStateMachine>().inputReader.controls.Controls.Enable();
    }

    IEnumerator CameraFinish()
    {
        collected = true;
        MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].enemyEncounterComplete[serializationId] = true;
        MainManager.Instance.SavePlayerInfo();
        player.GetComponent<PlayerStateMachine>().inputReader.controls.Controls.Disable();

        orbitCamera.SetFocusPoint(rewardSetPosition, new Vector2(20, 90));
        SetCompleted();
        yield return new WaitForSeconds(3);
        orbitCamera.ResetFocusToPlayer();
        player.GetComponent<PlayerStateMachine>().inputReader.controls.Controls.Enable();

    }

    override public void SetCollected(bool collected)
    {
        base.SetCollected(collected);
        if (this.collected)
        {
            Debug.Log("set completed");
            SetCompleted();
        }
    }

    void SetCompleted()
    {
        reward.transform.position = rewardSetPosition;
        mAnimation.Play();
    }
}
