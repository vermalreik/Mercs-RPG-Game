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

    public event Action OnUpdated;

    public List<Pokemon> Pokemons{
        get{
            return pokemons;
        }
        set{
            pokemons = value;
            OnUpdated?.Invoke();
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
            OnUpdated?.Invoke();
        }
        else
        {
            // TODO: Add to the PC once that's implemented
        }
    }

    public IEnumerator CheckForEvolutions()
    {
        foreach(var pokemon in pokemons)
        {
            var evolution = pokemon.CheckForEvolution();
            if(evolution != null)
            {
                yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} evolved into {evolution.EvolvesInto.Name}");
                pokemon.Evolve(evolution);
            }
        }

        OnUpdated?.Invoke();
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

}
