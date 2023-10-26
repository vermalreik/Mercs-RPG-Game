using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();

        // Load scriptable objects form code
        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        // This function will only load the objects that are inside a folder called 'Resources'

        foreach(var pokemon in pokemonArray)
        {
            if(pokemons.ContainsKey(pokemon.Name)) // you have to use "N" uppercase. If you use '.name' it will take the name of the scriptable object instead of 'Name' property from that class
            {
                Debug.Log($"There are two pokemons with the name {pokemon.Name}");
                continue;
            }
            pokemons[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if(!pokemons.ContainsKey(name))
        {
            Debug.Log($"Pokemon with name {name} not found in the database");
            return null;
        }

        return pokemons[name];
    }
}
