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

    public List<Pokemon> Pokemons{
        get{
            return pokemons;
        }
    }
    
    private void Start()
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

}
