using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField]  PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get;set; } // to store the pokemon we create in Setup()

    // For testing the battle system we will set these values from the Inspector
    // but later on we will start these dynamically when we start a battle

    // This function will create a Pokemon based from the base and level
    public void Setup()
    {
        Pokemon =  new Pokemon(_base, level);
        if(isPlayerUnit)
            GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
        else
            GetComponent<Image>().sprite = Pokemon.Base.FrontSprite;
    }
}
