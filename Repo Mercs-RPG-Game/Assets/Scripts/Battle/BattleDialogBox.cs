using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond; //30
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;


    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

     [SerializeField] Text yesText;
    [SerializeField] Text noText;

    Color highlightedColor;

    private void Start() {
        highlightedColor = GlobalSettings.i.HighlightedColor;
    }

    // Escribe el texto de una
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    // Escribe el texto letra a letra :D
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        //yield return new WaitForSeconds(1f);
        // :D Si hago WaitForSeconds(1f / 30) in one second it willshow 30 letters

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled; // "enabled" for texts
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled); // "SetActive" for game objects
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled); // "SetActive" for game objects
    }

    // Changues the color of the selectedAction text
    public void UpdateActionSelection(int selectedAction)
    {
        for(int i=0; i<actionTexts.Count; ++i)
        {
            if(i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    // Changues the color of the selectedMove text
    public void UpdateMoveSelection(int selectedMove, Move move) // el parametro Move es para el texto PP y Type
    {
        for(int i=0; i<moveTexts.Count; ++i)
        {
            if(i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();

        if(move.PP == 0)
            ppText.color = Color.red;
        else
            ppText.color = Color.black;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for(int i=0; i<moveTexts.Count; ++i)
        {
            if( i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if(yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }

}
