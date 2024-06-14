using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    public Animator animator { get; private set; }

    public float baseSpeed = .5f;

    private GameObject questionMark;
    private GameObject talkIcon;

    private GameObject mainCamera;

    public DialogObject dialog;

    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        questionMark = transform.Find("QuestionMark").gameObject;
        questionMark.SetActive(true);
        talkIcon = transform.Find("Talk").gameObject;
        talkIcon.SetActive(false);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");

        animator = GetComponentInChildren<Animator>();
        SwitchState(new NpcIdleState(this));
    }

    public override void Update()
    {
        base.Update();
        questionMark.transform.LookAt(mainCamera.transform);
        talkIcon.transform.LookAt(mainCamera.transform);

    }

    public void ReceiveDamge()
    {

    }

    public void InTalkingRange()
    {
        questionMark.SetActive(false);
        talkIcon.SetActive(true);
    }

    public void OutOfTalkingRange()
    {
        questionMark.SetActive(true);
        talkIcon.SetActive(false);
    }

    public void RotateTowardsplayer(Vector3 position)
    {
        StartCoroutine(_RotateTowardsPlayer(position));
    }

    IEnumerator _RotateTowardsPlayer(Vector3 position)
    {
        Quaternion lookRotation = Quaternion.LookRotation(transform.position-new Vector3(position.x, transform.position.y, position.z));

        Quaternion initialRotation = transform.rotation;
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);

            time += Time.deltaTime * 10f;

            yield return null;
        }

        SwitchState(new NpcTalkState(this));
    }

    public void StopTalking()
    {
        SwitchState(new NpcIdleState(this));
    }
}
