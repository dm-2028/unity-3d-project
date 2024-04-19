using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Dialog Object", menuName = "Dialog System")]
public class DialogObject : ScriptableObject
{
    public List<string> DialogText;

    public bool IsLoopable;
    public bool IsOrdered;

    protected string _dialogID;
    protected bool _hasCompleted;
    protected Queue<string> _orderedDialog;
    protected bool[] _readHistory;

    public bool HasNext()
    {
        if(_orderedDialog.Count > 0)
        {
            return true;
        }
        return false;
    }
    public string GetNextDialog()
    {
        if (IsOrdered)
        {
            if (_orderedDialog.Count > 0)
            {
                Debug.Log("returning " + _orderedDialog.Peek());
                return _orderedDialog.Dequeue();
            }

            else
            {
                if (IsLoopable)
                {
                    BuildTextQueue();
                    return _orderedDialog.Dequeue();
                }
                else
                {
                    BuildTextQueue();
                    Debug.Log("ondialogcomplete");
                    return null;
                }
            }
        }

        int randomIndex = Random.Range(0, DialogText.Count);
        return DialogText[randomIndex];
    }

    private void OnEnable()
    {
        if (IsOrdered)
        {
            BuildTextQueue();
        }
    }

    private void BuildTextQueue()
    {
        _orderedDialog = new Queue<string>();
        for(int i = 0; i < DialogText.Count; i++)
        {
            _orderedDialog.Enqueue(DialogText[i]);
        }
    }
}
