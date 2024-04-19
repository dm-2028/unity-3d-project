using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject dialogBox;

    private DialogUI dialogUI;

    private DialogObject currentDialog;
    public InputReader inputReader { get; private set; }

    public void ExitGame()
    {
        Time.timeScale = 1;
        MainManager.Instance.SavePlayerInfo();
        SceneManager.LoadScene(0);
    }

    public void Close()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void DisplayText(DialogObject dialog)
    {
        currentDialog = dialog;
        inputReader = GetComponent<InputReader>();
        inputReader.OnJumpPerformed += AdvanceText;
        dialogBox.SetActive(true);
        dialogUI = dialogBox.GetComponent<DialogUI>();
        GetNextDialog();
    }

    private void GetNextDialog()
    {
        string nextDialog = currentDialog.GetNextDialog();
        if (nextDialog != null)
        {
            dialogUI.ShowText(nextDialog, true);
        }
        else
        {
            CloseDialog();
        }
    }

    private void AdvanceText()
    {
        if (dialogUI.currentlyTyping)
        {
            dialogUI.currentlyTyping = false;
        }
        else
        {
            GetNextDialog();
        }
        
    }

    public void CloseDialog()
    {
        inputReader.OnJumpPerformed -= AdvanceText;
        inputReader = null;
        dialogBox.SetActive(false);
        dialogUI = null;
        currentDialog = null;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().StopTalking();
    }
}