using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField]
    Button newGameButton, continueButton, loadGameButton, nameBackButton, saveListBackButton, quitButton;

    Button[] buttons;
    Button[][] keyboard;

    [SerializeField]
    GameObject buttonPrefab;
    List<Button> saveFileButtons = new List<Button>();

    private int selectionIndex, rowIndex, keyIndex = 0, saveKeyIndex = 0;
    private int saveSelectionIndex = 0;
    string nameString = "";
    FileInfo[] files;

    [SerializeField]
    GameObject mainMenu, enterName, saveList, scrollContent;

    [SerializeField]
    Button Qbutton, Wbutton, Ebutton, Rbutton, Tbutton, Ybutton, Ubutton, Ibutton, Obutton, Pbutton,
        Abutton, Sbutton, Dbutton, Fbutton, Gbutton, Hbutton, Jbutton, Kbutton, Lbutton,
        Zbutton, Xbutton, Cbutton, Vbutton, Bbutton, Nbutton, Mbutton, BackButton, EnterButton;

    [SerializeField]
    TextMeshProUGUI nameText;

    int buttonHeight = 100;

    private bool verticalAxisDown = false, horizontalAxisDown = false;
    private float previousVerticalAxis = 0, previousHorizontalAxis = 0;
   
    Vector2 scrollBasePosition;

    private InputReader inputReader;

    // Start is called before the first frame update
    void Start()
    {
        enterName.SetActive(false);
        mainMenu.SetActive(true);
        saveList.SetActive(false);
        buttons = new[] { newGameButton, continueButton, loadGameButton, quitButton };
        keyboard = new Button[][] { new[] { Qbutton, Wbutton, Ebutton, Rbutton, Tbutton, Ybutton, Ubutton, Ibutton, Obutton, Pbutton },
                             new[]{ Abutton, Sbutton, Dbutton, Fbutton, Gbutton, Hbutton, Jbutton, Kbutton, Lbutton },
                             new[]{ Zbutton, Xbutton, Cbutton, Vbutton, Bbutton, Nbutton, Mbutton, BackButton, EnterButton},
                             new[]{ nameBackButton } };
        keyboard[rowIndex][keyIndex].Select();
        keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

        buttons[selectionIndex].Select();
        buttons[selectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        scrollBasePosition = ((RectTransform)scrollContent.transform).anchoredPosition;

        inputReader = GetComponent<InputReader>();
        inputReader.OnJumpPerformed += SelectButton;
        files = MainManager.Instance.GetAllFiles();
    }

    // Update is called once per frame
    void Update()
    {
        float directionVertical = 0, directionHorizontal = 0;


        float verticalAxis = inputReader.movement.y;
        float horizontalAxis = inputReader.movement.x;

        if (Mathf.Abs(verticalAxis) > Mathf.Abs(previousVerticalAxis) &&
            Mathf.Abs(verticalAxis) > .5f)
        {
            if (!verticalAxisDown)
            {
                verticalAxisDown = true;
                directionVertical = verticalAxis;
            }
        }
        if (Mathf.Abs(horizontalAxis) > Mathf.Abs(previousHorizontalAxis) &&
            Mathf.Abs(horizontalAxis) > .5f)
        {
            if (!horizontalAxisDown)
            {
                horizontalAxisDown = true;
                directionHorizontal = horizontalAxis;

            }
        }
        Debug.Log(directionVertical + " " + directionHorizontal + " " + rowIndex + " " + keyIndex);

        if (Mathf.Abs(directionVertical) > Mathf.Abs(directionHorizontal))
        {
            directionHorizontal = 0;
        }
        else
        {
            directionVertical = 0;
        }

        if (mainMenu.activeInHierarchy)
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
        else if (enterName.activeInHierarchy)
        {
            bool indexChanged = false;
            if (directionVertical < 0)
            {
                keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                if (rowIndex == keyboard.Length - 2)
                {
                    saveKeyIndex = keyIndex;
                }
                else if (rowIndex == keyboard.Length - 1)
                {
                    keyIndex = saveKeyIndex;
                }
                rowIndex = (rowIndex + 1) % keyboard.Length;
                if (keyIndex >= keyboard[rowIndex].Length)
                {
                    keyIndex = keyboard[rowIndex].Length - 1;
                }
                indexChanged = true;

            }
            else if (directionVertical > 0)
            {

                keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                if (rowIndex == 0)
                {
                    saveKeyIndex = keyIndex;
                }
                else if (rowIndex == keyboard.Length - 1)
                {
                    keyIndex = saveKeyIndex;
                }
                rowIndex = (rowIndex - 1) < 0 ? keyboard.Length - 1 : rowIndex - 1;
                indexChanged = true;

            }
            else if (directionHorizontal < 0)
            {
                keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                keyIndex = (keyIndex - 1) < 0 ? keyboard[rowIndex].Length - 1 : keyIndex - 1;
                indexChanged = true;

            }
            else if (directionHorizontal > 0)
            {
                keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                keyIndex = (keyIndex + 1) % keyboard[rowIndex].Length;
                indexChanged = true;

            }
            if (indexChanged)
            {
                if(keyIndex >= keyboard[rowIndex].Length)
                {
                    keyIndex = keyboard[rowIndex].Length - 1;
                }
                keyboard[rowIndex][keyIndex].Select();
                keyboard[rowIndex][keyIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;

            }

        }
        else if (saveList.activeInHierarchy)
        {
            bool indexChanged = false;
            if (directionVertical < 0)
            {
                saveFileButtons[saveSelectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                saveSelectionIndex = (saveSelectionIndex + 1) % saveFileButtons.Count;
                indexChanged = true;
            }
            else if (directionVertical > 0)
            {
                saveFileButtons[saveSelectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
                saveSelectionIndex = (saveSelectionIndex - 1) < 0 ? saveFileButtons.Count - 1 : saveSelectionIndex - 1;
                indexChanged = true;

            }

            if (indexChanged) 
            {
                if (saveSelectionIndex < 3)
                {
                    ((RectTransform)scrollContent.transform).anchoredPosition = scrollBasePosition;
                }
                else if (saveSelectionIndex < saveFileButtons.Count - 2)
                {
                    ((RectTransform)scrollContent.transform).anchoredPosition = new Vector2(0, scrollBasePosition.y + (buttonHeight * (saveSelectionIndex-2)));
                }
                else
                {
                    ((RectTransform)scrollContent.transform).anchoredPosition = new Vector2(0, scrollBasePosition.y + (buttonHeight * (saveFileButtons.Count - 4)));
                }
                saveFileButtons[saveSelectionIndex].Select();
                saveFileButtons[saveSelectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }

        }

        if (Mathf.Abs(verticalAxis) < Mathf.Abs(previousVerticalAxis))
        {
            verticalAxisDown = false;
        }
        if (Mathf.Abs(horizontalAxis) < Mathf.Abs(previousHorizontalAxis))
        {
            horizontalAxisDown = false;
        }
        previousVerticalAxis = verticalAxis;
        previousHorizontalAxis = horizontalAxis;



    }
    private void OnDestroy()
    {
        inputReader.OnJumpPerformed -= SelectButton;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("My Game");
    }

    public void NewGame()
    {
        mainMenu.SetActive(false);
        enterName.SetActive(true);
    }
    
    public void ContinueGame()
    {
        MainManager.Instance.LoadMostRecent();
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        mainMenu.SetActive(false);
        saveList.SetActive(true);
        BuildLoadMenu();
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        saveList.SetActive(false);
        enterName.SetActive(false);
    }

    void SelectButton()
    {
        if (mainMenu.activeInHierarchy)
        {
            buttons[selectionIndex].onClick.Invoke();
        }
        else if (enterName.activeInHierarchy)
        {
            keyboard[rowIndex][keyIndex].onClick.Invoke();
        }
        else if (saveList.activeInHierarchy)
        {
            saveFileButtons[saveSelectionIndex].onClick.Invoke();
        }
    }

    public void LoadSelectedFile(string path)
    {
        MainManager.Instance.LoadPlayerInfo(path);
        SceneManager.LoadScene(1);
    }

    public void AddLetter(string letter)
    {
        nameString += letter;
        nameText.text = nameString;
    }
    public void DeleteLetter()
    {
        nameString = nameString.Remove(nameString.Length - 1);
        nameText.text = nameString;
    }

    public void BuildLoadMenu()
    {
        saveFileButtons.Add(saveListBackButton);
        for(int i = 0; i < files.Length; i++)
        {
            GameObject newObject = GameObject.Instantiate(buttonPrefab, scrollContent.transform);
            RectTransform objectRect = (RectTransform)newObject.transform;
            TextMeshProUGUI fileText = newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Button fileButton = newObject.GetComponent<Button>();

            string json = File.ReadAllText(files[i].FullName);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            fileText.text = "Name: " + data.displayName + "  Date: " + files[i].LastWriteTime;
            saveFileButtons.Add(fileButton);
            objectRect.sizeDelta = new(700, buttonHeight);
            objectRect.anchoredPosition = new(0, 150-(buttonHeight * (i+1)));
            string fileName = files[i].FullName;
            fileButton.onClick.AddListener(() => 
                LoadSelectedFile(fileName)
            );
            if (saveSelectionIndex == i)
            {
                fileButton.Select();
                saveFileButtons[saveSelectionIndex].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }
    }

    public void EnterClicked()
    {
        MainManager.Instance.CreateNewFile(nameString);

        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
