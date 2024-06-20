using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu, dialogBox, gameOver, settingsMenu, totalsMenu;

    
    Button[] pauseButtons, gameOverButtons, settingsButtons, totalsButtons;

    [SerializeField]
    Button mainMenuPause, mainMenuGameOver, continueButton, close, totalsBack, settingsBack, settings, totals, volume;

    [SerializeField]
    Slider volumeSlider;

    private DialogUI dialogUI;

    private DialogObject currentDialog;

    private bool enterPressed;

    private int selectionIndex = 0;

    private bool verticalAxisDown = false, horizontalAxisDown = false;
    private float previousVerticalAxis = 0, previousHorizontalAxis = 0;

    public InputReader inputReader { get; private set; }

    private void Start()
    {
        pauseMenu.SetActive(false);
        dialogBox.SetActive(false);
        gameOver.SetActive(false);
        settingsMenu.SetActive(false);
        totalsMenu.SetActive(false);
        pauseButtons = new[] { close, settings, totals, mainMenuPause };
        gameOverButtons = new[] { continueButton, mainMenuGameOver };
        settingsButtons = new[] { settingsBack, volume };
        totalsButtons = new[] { totalsBack };
    }

    private void Update()
    {
        Debug.Log("Update");
        if (pauseMenu.activeInHierarchy || gameOver.activeInHierarchy || settingsMenu.activeInHierarchy)
        {
            float directionVertical = 0, directionHorizontal = 0;


            float verticalAxis = Input.GetAxis("Vertical");
            float horizontalAxis = Input.GetAxis("Horizontal");

            if (Mathf.Abs(verticalAxis) > Mathf.Abs(previousVerticalAxis))
            {
                if (!verticalAxisDown)
                {
                    verticalAxisDown = true;
                    directionVertical = verticalAxis;
                }
            }
            if (Mathf.Abs(horizontalAxis) > Mathf.Abs(previousHorizontalAxis))
            {
                if (!horizontalAxisDown)
                {
                    horizontalAxisDown = true;
                    directionHorizontal = horizontalAxis;

                }
            }

            if (pauseMenu.activeInHierarchy)
            {
                HandleSelection(directionVertical, pauseButtons);
            }else if (gameOver.activeInHierarchy)
            {
                HandleSelection(directionVertical, gameOverButtons);

            }else if (settingsMenu.activeInHierarchy)
            {
                HandleSelection(directionVertical, settingsButtons);
                HandleVolume(directionHorizontal);
            }
            if (Mathf.Abs(verticalAxis) < Mathf.Abs(previousVerticalAxis))
            {
                verticalAxisDown = false;
            }
            previousVerticalAxis = verticalAxis;

        }
    }

    private void HandleSelection(float directionVertical, Button[] buttons)
    {
        Debug.Log("handle selection " + directionVertical);
        bool indexChanged = false;
        if (directionVertical < 0)
        {
            buttons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            selectionIndex = (selectionIndex + 1) % buttons.Length;
            indexChanged = true;

        }
        else if (directionVertical > 0)
        {
            buttons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            selectionIndex = (selectionIndex - 1) < 0 ? buttons.Length - 1 : selectionIndex - 1;
            indexChanged = true;

        }
        if (indexChanged)
        {
            buttons[selectionIndex].Select();
            buttons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }
    }

    private void HandleVolume(float directionHorizontal)
    {
        Debug.Log("handle volume " + directionHorizontal);
        if(directionHorizontal < 0)
        {
            volumeSlider.value = Mathf.Max(0f, volumeSlider.value - directionHorizontal * .01f * Time.deltaTime);
        }else if(directionHorizontal > 0)
        {
            volumeSlider.value = Mathf.Min(1f, volumeSlider.value + directionHorizontal * .01f * Time.deltaTime);

        }
    }

    void ResetIndex(Button[] buttons)
    {
        selectionIndex = 0;
        buttons[selectionIndex].Select();
    }


    void SelectButton()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseButtons[selectionIndex].onClick.Invoke();
        }
        else if (gameOver.activeInHierarchy)
        {
            gameOverButtons[selectionIndex].onClick.Invoke();
        }else if (settingsMenu.activeInHierarchy)
        {
            if(selectionIndex == 0)
            {
                settingsButtons[selectionIndex].onClick.Invoke();
            }
        }
    }
    public void ExitGame()
    {
        inputReader.OnJumpPerformed -= SelectButton;
        inputReader = null;
        Time.timeScale = 1;
        MainManager.Instance.SavePlayerInfo();
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        
        inputReader = GetComponent<InputReader>();
        inputReader.OnJumpPerformed += SelectButton;

        pauseMenu.gameObject.SetActive(true);
        selectionIndex = 0;
        pauseButtons[selectionIndex].Select();
        pauseButtons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
    }

    public void UnPauseGame()
    {
        inputReader.OnJumpPerformed -= SelectButton;
        inputReader = null;

        Time.timeScale = 1;
        pauseButtons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        GameObject.Find("GameManager").GetComponent<GameManager>().gamePaused = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().returnFromPause = true;
        pauseMenu.gameObject.SetActive(false);
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        inputReader = GetComponent<InputReader>();
        inputReader.OnJumpPerformed += SelectButton;

        gameOver.gameObject.SetActive(true);

        selectionIndex = 0;
        gameOverButtons[selectionIndex].Select();
        gameOverButtons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
    }
    public void Continue()
    {
        inputReader = GetComponent<InputReader>();
        inputReader.OnJumpPerformed += SelectButton;
        inputReader = null;
        gameOver.gameObject.SetActive(false);
        MainManager.Instance.LoadMostRecent();
        MainManager.Instance.health = 10;
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
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

    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        ResetIndex(settingsButtons);
    }

    public void OpenTotals()
    {
        pauseMenu.SetActive(false);
        totalsMenu.SetActive(true);
        ResetIndex(totalsButtons);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        totalsMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }
    private void GetNextDialog()
    {
        Dialog nextDialog = currentDialog.GetNextDialog();
        if (nextDialog != null)
        {
            dialogUI.ShowText(nextDialog.dialogText, true);
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