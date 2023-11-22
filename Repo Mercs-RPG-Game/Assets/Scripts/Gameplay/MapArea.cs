using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsInWater;

    [HideInInspector]
    [SerializeField] int totalChance = 0;

    [HideInInspector]
    [SerializeField] int totalChanceWater = 0;

    // When we start the game from build this function will not be called, so we have to call this code "CalculateChancePercentage()" from Start function also
    private void OnValidate() // This function will be call whenever we make a change in the Inspector
    {
        CalculateChancePercentage();
    }

    private void Start()
    {
        CalculateChancePercentage();
    }

    void CalculateChancePercentage()
    {
        totalChance = -1;
        totalChanceWater = -1;

        if(wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.chanceLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance = totalChance + record.chancePercentage;
            }
        }

        if(wildPokemonsInWater.Count > 0)
        {
            totalChanceWater = 0;
            foreach (var record in wildPokemonsInWater)
            {
                record.chanceLower = totalChanceWater;
                record.chanceUpper = totalChanceWater + record.chancePercentage;

                totalChanceWater = totalChanceWater + record.chancePercentage;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = (trigger == BattleTrigger.LongGrass) ? wildPokemons : wildPokemonsInWater;

        int randVal = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1); // + 1 because the second value is exclusive
        
        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        wildPokemon.Init();
        return wildPokemon;
    }
}

[System.Serializable]
// When you specify the List to file Pokemon,
//it's really important that the sum of all the chance percentages should be 100
public class PokemonEncounterRecord // Data container
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower {get; set; }
    public int chanceUpper {get; set; }
}
