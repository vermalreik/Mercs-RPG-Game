using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond; //45

    public event Action OnShowDialog;
     public event Action OnDialogFinished;

    // Singleton pattern
    public static DialogManager Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
    // :D singleton hasta aki
    // be careful when you use it because it's very easy to access the Instance of DialogManager
    // and to create unwanted dependencies to it from different classes

    public bool IsShowing{ get; private set; } 

    public IEnumerator ShowDialogText(string text, bool waitForInput=true, bool autoClose=true,
        List<string> choices=null, Action<int> onChoiceSelected=null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        AudioManager.i.PlaySfx(AudioId.UISelect);
        yield return TypeDialog(text);
        if(waitForInput)
        {
            yield return new WaitUntil( () => Input.GetKeyDown(KeyCode.E));
        }

        if(choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if(autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices=null,
        Action<int> onChoiceSelected=null)
    {
        // wait for the frame to end before we start the dialogue will help us avoid lots of problems
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);
        
        foreach (var line in dialog.Lines)
        {
            AudioManager.i.PlaySfx(AudioId.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }

        if(choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    public void HandleUpdate()
    {
        //
    }

    // Escribe el texto letra a letra :D
    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
}
