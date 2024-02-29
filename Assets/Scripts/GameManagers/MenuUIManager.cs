using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField]
    Button newGameButton, continueButton, loadGameButton;

    Button[] buttons;
    Button[][] keyboard;

    private int selectionIndex, rowIndex, keyIndex = 0;
    string nameString = "";
    bool enterPressed;

    [SerializeField]
    GameObject mainMenu, enterName;

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
        //highScoreText.text = "High Score: " + MainManager.Instance.highScore;
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

    }

    public void LoadGame()
    {
        
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

    public void EnterClicked()
    {
        SaveData data = new SaveData();

        data.name = nameString;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/" + name + Time.time + ".json", json);

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
