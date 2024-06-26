using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu, dialogBox, gameOver, settingsMenu, totalsMenu, scrollContent;

    [SerializeField]
    TextMeshProUGUI beansText, dragonFruitText;
    Button[] pauseButtons, gameOverButtons, settingsButtons, totalsButtons;

    [SerializeField]
    Button mainMenuPause, mainMenuGameOver, continueButton, close, totalsBack, settingsBack, settings, totals, volume;

    [SerializeField]
    Slider volumeSlider;

    [SerializeField]
    GameObject levelDataPrefab;

    private DialogUI dialogUI;

    private DialogObject currentDialog;

    private int selectionIndex = 0;

    private bool verticalAxisDown = false;
    private float previousVerticalAxis = 0;
    private float horizontalAxisTimer = 1;

    public InputReader inputReader { get; private set; }

    OrbitCamera mainCamera;

    int levelInfoHeight = 100;

    Vector2 scrollBasePosition;

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
        scrollBasePosition = ((RectTransform)scrollContent.transform).anchoredPosition;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<OrbitCamera>();
    }

    private void Update()
    {
        float directionVertical = 0, directionHorizontal = 0;

        float verticalAxis = inputReader.movement.y;
        float horizontalAxis = inputReader.movement.x;

        if (Mathf.Abs(verticalAxis) > Mathf.Abs(previousVerticalAxis))
        {
            if (!verticalAxisDown)
            {
                verticalAxisDown = true;
                directionVertical = verticalAxis;
            }
        }

        if (pauseMenu.activeInHierarchy || gameOver.activeInHierarchy || settingsMenu.activeInHierarchy)
        {
            if (horizontalAxisTimer <= 0)
            {
                horizontalAxisTimer = 1;
                directionHorizontal = horizontalAxis;

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
                volumeSlider.value = Mathf.Clamp(volumeSlider.value + directionHorizontal * .01f, 0f, 1f);

            }
            horizontalAxisTimer -= 100 * Time.unscaledDeltaTime;
        }else if (totalsMenu.activeInHierarchy)
        {
            if (directionVertical < 0
                && ((RectTransform)scrollContent.transform).anchoredPosition.y < (scrollContent.transform.childCount-3)*levelInfoHeight)
            {
                ((RectTransform)scrollContent.transform).anchoredPosition += new Vector2(0, levelInfoHeight);
            }
            else if (directionVertical > 0
                && ((RectTransform)scrollContent.transform).anchoredPosition.y > scrollBasePosition.y+levelInfoHeight)
            {
                ((RectTransform)scrollContent.transform).anchoredPosition -= new Vector2(0, levelInfoHeight);

            }
        }
        if (Mathf.Abs(verticalAxis) < Mathf.Abs(previousVerticalAxis))
        {
            verticalAxisDown = false;
        }
        previousVerticalAxis = verticalAxis;

    }

    private void HandleSelection(float directionVertical, Button[] buttons)
    {
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

    void ResetIndex(Button[] buttons)
    {
        foreach (Button button in buttons)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        }
        selectionIndex = 0;
        buttons[selectionIndex].Select();
        buttons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
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
        }else if (totalsMenu.activeInHierarchy)
        {
            totalsButtons[selectionIndex].onClick.Invoke();
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().inputReader.enabled = false;

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
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().inputReader.enabled = true;

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
        BuildTotalsMenu();
        ResetIndex(totalsButtons);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        totalsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        ResetIndex(pauseButtons);
    }
    private void GetNextDialog()
    {
        Dialog nextDialog = currentDialog.GetNextDialog();
        if (nextDialog != null)
        {
            if(nextDialog.cameraFocusPosition != null || nextDialog.cameraRotation != null)
            {
                mainCamera.SetFocusPoint(nextDialog.cameraFocusPosition, nextDialog.cameraRotation);
            }
            dialogUI.ShowText(nextDialog.dialogText, true);
        }
        else
        {
            CloseDialog();
            mainCamera.ResetFocusToPlayer();
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

    public void BuildTotalsMenu()
    {
        for (int i = 0; i < MainManager.Instance.levelDataObjects.Length; i++)
        {
            GameObject newListItem = Instantiate(levelDataPrefab, scrollContent.transform);
            RectTransform itemRect = (RectTransform)newListItem.transform;
            TextMeshProUGUI levelNameText = newListItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI BeanCountText = newListItem.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI BeanTotalText = newListItem.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI FruitCountText = newListItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI FruitTotalText = newListItem.transform.GetChild(6).GetComponent<TextMeshProUGUI>();

            levelNameText.text = MainManager.Instance.levelDataObjects[i].levelName;
            BeanCountText.text = MainManager.Instance.levelData[i].coffeeBeanCollected.Count(o => o).ToString();
            BeanTotalText.text = "/" + MainManager.Instance.levelDataObjects[i].totalBeans.ToString();

            bool[] partialFruitCollected = MainManager.Instance.levelData[i].partialFruitCollected;
            if(partialFruitCollected.Length == 0 || partialFruitCollected.All(o => o))
            {
                FruitCountText.text = MainManager.Instance.levelData[i].fruitCollected.Count(o => o).ToString();
            }
            else
            {
                FruitCountText.text = MainManager.Instance.levelData[i].fruitCollected.Count(o => o).ToString() + " (" + partialFruitCollected.Count(o => o).ToString() + "/3)";
            }

            FruitTotalText.text = "/" + MainManager.Instance.levelDataObjects[i].totalFruit.ToString();
            itemRect.sizeDelta = new(950, levelInfoHeight);
            itemRect.anchoredPosition = new(0, 200 - (levelInfoHeight * i));
        }
    }

    public void UpdateCollectableText()
    {
        beansText.text = "Beans: " + MainManager.Instance.beans.ToString();

        bool[] partialFruitCollected = MainManager.Instance.levelData[MainManager.Instance.currentLevelIndex].partialFruitCollected;
        if (partialFruitCollected.Length == 0 || partialFruitCollected.All(o => o))
        {
            dragonFruitText.text = "Dragon Fruit: " + MainManager.Instance.totalDragonFruit.ToString();
        }
        else
        {
            dragonFruitText.text = "Dragon Fruit: (" + partialFruitCollected.Count(o => o) + "/3) " + MainManager.Instance.totalDragonFruit.ToString();
        }
    }
}