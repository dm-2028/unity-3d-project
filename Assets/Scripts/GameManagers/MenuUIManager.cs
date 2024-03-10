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
    Button newGameButton, continueButton, loadGameButton;

    Button[] buttons;
    Button[][] keyboard;

    [SerializeField]
    GameObject buttonPrefab;
    List<Button> saveFileButtons = new List<Button>();

    private int selectionIndex, rowIndex, keyIndex = 0;
    private int saveSelectionIndex = 0;
    string nameString = "";
    bool enterPressed;

    [SerializeField]
    GameObject mainMenu, enterName, saveList;

    [SerializeField]
    Button Qbutton, Wbutton, Ebutton, Rbutton, Tbutton, Ybutton, Ubutton, Ibutton, Obutton, Pbutton,
        Abutton, Sbutton, Dbutton, Fbutton, Gbutton, Hbutton, Jbutton, Kbutton, Lbutton,
        Zbutton, Xbutton, Cbutton, Vbutton, Bbutton, Nbutton, Mbutton, BackButton, EnterButton;

    [SerializeField]
    TextMeshProUGUI nameText;


    // Start is called before the first frame update
    void Start()
    {
        enterName.SetActive(false);
        mainMenu.SetActive(true);
        saveList.SetActive(false);
        buttons = new[] { newGameButton, continueButton, loadGameButton };
        keyboard = new Button[][] { new[] { Qbutton, Wbutton, Ebutton, Rbutton, Tbutton, Ybutton, Ubutton, Ibutton, Obutton, Pbutton },
                             new[]{ Abutton, Sbutton, Dbutton, Fbutton, Gbutton, Hbutton, Jbutton, Kbutton, Lbutton },
                             new[]{ Zbutton, Xbutton, Cbutton, Vbutton, Bbutton, Nbutton, Mbutton, BackButton, EnterButton} };
        keyboard[rowIndex][keyIndex].Select();
        Debug.Log(keyboard[0].Length + keyboard[1].Length + keyboard[2].Length);
        buttons[selectionIndex].Select();
    }

    // Update is called once per frame
    void Update()
    {
        float directionVertical = 0, directionHorizontal = 0;

        enterPressed = Input.GetButtonDown("Jump");
        if (enterPressed)
        {
            Debug.Log("enter pressed");
            if (mainMenu.activeInHierarchy)
            {
                buttons[selectionIndex].onClick.Invoke();
            }
            else if (enterName.activeInHierarchy)
            {
                keyboard[rowIndex][keyIndex].onClick.Invoke();
            }else if (saveList.activeInHierarchy)
            {
                saveFileButtons[saveSelectionIndex].onClick.Invoke();
            }
            enterPressed = false;
            return;
        }
        if (Input.anyKeyDown) 
        {
            directionVertical = Input.GetAxis("Vertical");
            directionHorizontal = Input.GetAxis("Horizontal");
        }
        if (mainMenu.activeInHierarchy)
        {
            if (directionVertical < 0)
            {
                selectionIndex = (selectionIndex + 1) % buttons.Length;
            }
            else if (directionVertical > 0)
            {
                selectionIndex = (selectionIndex - 1) < 0 ? buttons.Length - 1 : selectionIndex - 1;
            }
            buttons[selectionIndex].Select();
        }else if (enterName.activeInHierarchy)
        {
            if (directionVertical < 0)
            {
                rowIndex = (rowIndex + 1) % buttons.Length;
                if(keyIndex >= keyboard[rowIndex].Length)
                {
                    keyIndex = keyboard[rowIndex].Length - 1;
                }
            }
            else if (directionVertical > 0)
            {
                rowIndex = (rowIndex - 1) < 0 ? keyboard.Length - 1 : rowIndex - 1;
            }
            if(directionHorizontal < 0)
            {
                keyIndex = (keyIndex - 1) < 0 ? keyboard[rowIndex].Length - 1 : keyIndex - 1;
            }
            else if(directionHorizontal > 0)
            {
                keyIndex = (keyIndex + 1) % keyboard[rowIndex].Length;
            }
            if (keyIndex >= keyboard[rowIndex].Length)
            {
                keyIndex = keyboard[rowIndex].Length - 1;
            }

            keyboard[rowIndex][keyIndex].Select();
        }else if (saveList.activeInHierarchy){
            if(directionVertical < 0)
            {
                saveSelectionIndex = (saveSelectionIndex + 1) % saveFileButtons.Count;
            }
            else if (directionVertical > 0)
            {
                saveSelectionIndex = (saveSelectionIndex - 1) < 0 ? saveFileButtons.Count - 1 : saveSelectionIndex - 1;
            }
            Debug.Log("selecting " + saveSelectionIndex);
            Debug.Log(saveFileButtons[saveSelectionIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            //saveFileButtons[saveSelectionIndex].Select();
            saveList.transform.GetChild(0).transform.GetChild(saveSelectionIndex).GetComponent<Button>().Select();
        }
      
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
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

    public void LoadSelectedFile(string path)
    {
        MainManager.Instance.LoadPlayerInfo(path);
        SceneManager.LoadScene(1);
    }

    public void AddLetter(string letter)
    {
        Debug.Log("Add Letter " + letter);
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
        FileInfo[] files = MainManager.Instance.GetAllFiles();
        GameObject content = saveList.transform.GetChild(0).gameObject;
        Debug.Log("content " + content);
        for(int i = 0; i < files.Length; i++)
        {
            Debug.Log(files[i].Name);
            GameObject newObject = GameObject.Instantiate(buttonPrefab);
            newObject.transform.SetParent(content.transform);
            RectTransform objectRect = (RectTransform)newObject.transform;
            TextMeshProUGUI fileText = newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Button fileButton = newObject.GetComponent<Button>();
            fileText.text = files[i].Name;
            saveFileButtons.Add(fileButton);
            objectRect.sizeDelta = new(700, 200);
            objectRect.anchoredPosition = new(0, 150-(200 * i));
            fileButton.onClick.AddListener(delegate () { this.LoadSelectedFile(files[i].FullName); });
            if (saveSelectionIndex == i)
            {
                fileButton.Select();
            }
        }
        for(int i = 0; i < saveFileButtons.Count; i++)
        {
            Debug.Log(i + "save file buttons " + saveFileButtons[i].ToString());
        }
    }

    public void EnterClicked()
    {
        SaveData data = new SaveData();

        data.saveFileName = nameString + Time.time;

        Debug.Log(data.saveFileName + " is the file name");
        MainManager.Instance.SavePlayerInfo(data);

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
