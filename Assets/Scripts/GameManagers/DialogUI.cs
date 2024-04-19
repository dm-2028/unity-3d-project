using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogUI : MonoBehaviour
{
    public event System.Action TypingTextEnded;

    public TextMeshProUGUI dialogBoxText;
    public TextMeshProUGUI advanceIcon;

    public float typeTextDelay = 0.05f;
    public bool currentlyTyping { get; set; } = false;

    float iconTime;
    float blinkTime = .5f;
    private void Update()
    {
        if (currentlyTyping)
        {
            advanceIcon.transform.gameObject.SetActive(false);
        }
        else
        {
            iconTime += Time.deltaTime;

            if (iconTime >= blinkTime)
            {
                iconTime -= blinkTime;

                advanceIcon.transform.gameObject.SetActive(!advanceIcon.transform.gameObject.activeInHierarchy);
            }
        }
    }
    public void ShowText(string text, bool shouldType)
    {
        gameObject.SetActive(true);
        if (shouldType)
        {
            StartCoroutine(TypeText(text));
            
        }
        else
        {
            dialogBoxText.text = text;
        }
    }

    private IEnumerator TypeText(string text)
    {
        currentlyTyping = true;
        string fullText = text;
        string currentText;
        for(int i = 0; i < fullText.Length +1; i++)
        {
            if(currentlyTyping == false)
            {
                dialogBoxText.text = fullText;
                break;
            }
            currentText = fullText.Substring(0, i);
            dialogBoxText.text = currentText;
            yield return new WaitForSeconds(typeTextDelay);
        }
        currentlyTyping = false;

        TypingTextEnded?.Invoke();
        yield return null;
    }
}
