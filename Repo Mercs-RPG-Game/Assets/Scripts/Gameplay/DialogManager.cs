using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond; //45

    public event Action OnShowDialog;
     public event Action OnCloseDialog;

    // Singleton pattern
    public static DialogManager Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
    // :D singleton hasta aki
    // be careful when you use it because it's very easy to access the Instance of DialogManager
    // and to create unwanted dependencies to it from different classes

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    public IEnumerator ShowDialog(Dialog dialog)
    {
        // wait for the frame to end before we start the dialogue will help us avoid lots of problems
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        //dialogText.text = dialog.Lines[0];
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        if( ((Input.GetKeyDown(KeyCode.Z)) || (Input.GetKeyDown(KeyCode.E))) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    // Escribe el texto letra a letra :D
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
