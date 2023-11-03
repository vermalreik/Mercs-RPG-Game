using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // :D lo puedes importar rapidamente usando "CTRL" + "." sobre "where"
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;
    // if the list is not visible in the Inspector even if we made it [SerializeField]
    // You have to do add [System.Serializable] on top of Pokemon.cs class, so it will be shown in the inspector
    // it's because in class Pokemon, the variables: "PokemonBase" and "Level" are not Serialize Fields

    public event Action onUpdated;

    public List<Pokemon> Pokemons{
        get{
            return pokemons;
        }
        set{
            pokemons = value;
            onUpdated?.Invoke();
        }
    }
    
    private void Awake()
    {
        foreach(var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault(); // it will return null in case all the pokemons are fainted
    }

    public void AddPokemon(Pokemon newPokemon)
    {
        if(pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            onUpdated?.Invoke();
        }
        else
        {
            // TODO: Add to the PC once that's implemented
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

}
