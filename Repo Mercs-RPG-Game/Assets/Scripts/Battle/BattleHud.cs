using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;

    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon;
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon)
    {
        if(_pokemon != null)
        {
            _pokemon.OnHPChanged -= UpdateHP; // unsuscribe
            _pokemon.OnStatusChanged -= SetStatusText;
        }

        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        SetLevel();
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        // We need to Normalize the current HP of the Pokemon
        // We can do that by dividing the HP of the Pokemon by the Max HP
        // Make sure to convert it into float, since both HP and MaxHp are integers
        SetExp();
    
        statusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor},
            {ConditionID.brn, brnColor},
            {ConditionID.slp, slpColor},
            {ConditionID.par, parColor},
            {ConditionID.frz, frzColor}
        };
        
        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;
        _pokemon.OnHPChanged += UpdateHP;
    
    }

    void SetStatusText()
    {
        if(_pokemon.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper();
            statusText.color = statusColors[_pokemon.Status.Id];
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _pokemon.Level;
    }

    public void SetExp()
    {
        if(expBar == null) return; // This is important because only the Player Hud will have the ExpBar

        float normalizedExp = GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if(expBar == null) yield break; // This is important because only the Player Hud will have the ExpBar

        if(reset)
            expBar.transform.localScale = new Vector3(0, 1, 1);

        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }

    // Calculate the Normalized Exp of the Pokemon
    float GetNormalizedExp()
    {
        int currLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelExp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        // Formula to Normalize our current exp
        float normalizedExp = (float) (_pokemon.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp); // "Mathf.Clamp01()" in order to make sure that the normalized Exp is always between 0 and 1
    }

    public void UpdateHP() // Now since this is a normal function we can directly attach it to the OnHPChanged Event
    {
        StartCoroutine(UpdateHPAsync());;
    }

    public IEnumerator UpdateHPAsync() // because Coroutines are asynchronous
    {
        yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
    }

    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }
}
